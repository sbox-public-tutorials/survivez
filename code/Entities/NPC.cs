using Sandbox;
using System.Linq;
using System;
using survivez.NavMeshAgent;

namespace survivez.Entities
{
    public abstract partial class NPC : AnimEntity
    {
        public string Model { get; set; } = "models/citizen/citizen.vmdl";

        public float MaxHealth { get; set; } = 10.0f;
        public bool CanDie { get; set; } = true;

        public bool IsAlive { get => CanDie && Health > 0; }

        public float Speed { get; private set; }

        private DamageInfo lastDamage;
		public NavMeshPath Agent;

		public static Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;

		Vector3 InputVelocity;
		Vector3 LookDir;

		private Vector3? Destination { get => Agent.TargetEndPosition; }

        [ServerCmd("npc_clear")]
        public static void NPCClear()
        {
            foreach (var npc in All.OfType<NPC>().ToArray())
                npc.Delete();
        }

        public NPC() { }

        public virtual void Init() {}
        public virtual void Think()
		{
			if ( Destination != null )
			{
				float dist = Destination.Value.Distance( Position );
				DebugOverlay.Text( Position, $"{dist}", 0.01f );
				if ( dist <= 100.0f )
				{
					Log.Info($"Clear Destination {dist}");
					ClearDestination();
				}
			}

			if (Destination == null)
			{
                Log.Info("Set Destination");
				SetDestination( Position + new Vector3( Rand.Float(-25, 50), Rand.Float(-25, 50), 0 ) );
			}
		}
        public virtual void OnDeath() {}
        public virtual void OnDamaged( DamageInfo info ) {}

        public void ClearDestination()
		{
			Agent.TargetEndPosition = null;
		}
        public void SetDestination( Vector3? destination )
		{
			if (destination != null)
				DebugOverlay.Line( Position, destination.Value, Color.Magenta, 10.0f );
			Agent.TargetEndPosition = destination;
        }

        public override void Spawn()
        {
            base.Spawn();

            Health = MaxHealth;
            SetModel(Model);
            EyePos = Position + Vector3.Up * 64;
            CollisionGroup = CollisionGroup.Player;
            SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );

			EnableHitboxes = true;

			Speed = Rand.Float( 100, 300 );

            Init();
        }

        [Event.Tick.Server]
        public void Tick()
        {
            using var _a = Sandbox.Debug.Profile.Scope( "NpcTest::Tick" );

			InputVelocity = 0;

			using var _b = Sandbox.Debug.Profile.Scope( "Steer" );

			Agent.Update( Position );

			if ( !Agent.Output.Finished )
			{
				InputVelocity = Agent.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed );
			}

			if ( NavMeshSettings.nav_drawpath )
			{
				Agent.DebugDrawPath();
			}

			using ( Sandbox.Debug.Profile.Scope( "Move" ) )
			{
				Move( Time.Delta );
			}

			var walkVelocity = Velocity.WithZ( 0 );
			if ( walkVelocity.Length > 0.5f )
			{
				var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
				var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
				Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
			}

			using ( Sandbox.Debug.Profile.Scope( "Set Anim Vars" ) )
			{
				LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 1.0f );
				SetAnimLookAt( "lookat_pos", EyePos + LookDir );
				SetAnimLookAt( "aimat_pos", EyePos + LookDir );
				SetAnimFloat( "aimat_weight", 0.5f );
			}

			using ( Sandbox.Debug.Profile.Scope( "Set Anim Vars" ) )
			{
				SetAnimBool( "b_grounded", true );
				SetAnimBool( "b_noclip", false );
				SetAnimBool( "b_swim", false );

				var forward = Vector3.Dot( Rotation.Forward, Velocity.Normal );
				var sideward = Vector3.Dot( Rotation.Right, Velocity.Normal );
				var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();
				SetAnimFloat( "move_direction", angle );

				SetAnimFloat( "wishspeed", Velocity.Length * 1.5f );
				SetAnimFloat( "walkspeed_scale", 1.0f / 10.0f );
				SetAnimFloat( "runspeed_scale", 1.0f / 320.0f );
				SetAnimFloat( "duckspeed_scale", 1.0f / 80.0f );
			}

            Think();
        }

        protected virtual void Move( float timeDelta )
		{
			var bbox = BBox.FromHeightAndRadius( 64, 4 );
			MoveHelper move = new( Position, Velocity );
			move.MaxStandableAngle = 50;
			move.Trace = move.Trace.Ignore( this ).Size( bbox );

			if ( !Velocity.IsNearlyZero( 0.001f ) )
			{
				using ( Sandbox.Debug.Profile.Scope( "TryUnstuck" ) )
					move.TryUnstuck();

				using ( Sandbox.Debug.Profile.Scope( "TryMoveWithStep" ) )
					move.TryMoveWithStep( timeDelta, 30 );
			}

			using ( Sandbox.Debug.Profile.Scope( "Ground Checks" ) )
			{
				var tr = move.TraceDirection( Vector3.Down * 10.0f );

				if ( move.IsFloor( tr ) )
				{
					GroundEntity = tr.Entity;

					if ( !tr.StartedSolid )
					{
						move.Position = tr.EndPos;
					}

					if ( InputVelocity.Length > 0 )
					{
						var movement = move.Velocity.Dot( InputVelocity.Normal );
						move.Velocity = move.Velocity - movement * InputVelocity.Normal;
						move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
						move.Velocity += movement * InputVelocity.Normal;

					}
					else
					{
						move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
					}
				}
				else
				{
					GroundEntity = null;
					move.Velocity += Vector3.Down * 900 * timeDelta;
					Sandbox.Debug.Draw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
				}
			}

			Position = move.Position;
			Velocity = move.Velocity;
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( !IsAlive )
				return;

			lastDamage = info;

			Health -= info.Damage;

            OnDamaged(info);

            if (!IsAlive)
				OnKilled();
		}

		public override void OnKilled()
		{
			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );

			base.OnKilled();

            OnDeath();
		}
    }
} // namespace survivez.Entities

