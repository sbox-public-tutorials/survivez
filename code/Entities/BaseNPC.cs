using Sandbox;
using System.Linq;

namespace survivez.Entities
{
	// BaseNPC holds the movement logic, nothing about the AI and such.
	public abstract partial class BaseNPC : AnimEntity
	{
		public string Model { get; set; } = "models/citizen/citizen.vmdl";

		public float MaxHealth { get; set; } = 10.0f;
		public bool CanDie { get; set; } = true;

		public bool IsAlive { get => CanDie && Health > 0; }

		public float Speed { get; private set; }

		NavPath Path = new NavPath();
		public NavSteer Steer;

		Vector3 InputVelocity;

		Vector3 LookDir;


		[ServerCmd( "npc_clear" )]
		public static void NPCClear()
		{
			foreach ( var npc in All.OfType<NPC>().ToArray() )
				npc.Delete();
		}

		public BaseNPC()
		{
		}

		public override void Spawn()
		{
			base.Spawn();

			Health = MaxHealth;
			SetModel( Model );
			EyePos = Position + Vector3.Up * 64;
			CollisionGroup = CollisionGroup.Player;
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );

			EnableHitboxes = true;

			Speed = Rand.Float( 100, 300 );
			Init();
		}

		public virtual void Init() { }
		public virtual void Think()
		{
			// if (!Agent.HasPath)
			// {

			// // 	using(Sandbox.Debug.Profile.Scope( "NpcTest::Think::Looking For New Destination" ))
			// // 	{
			// 		SetDestination( Position + new Vector3( Rand.Float(-25, 50) * 10, Rand.Float(-25, 50) * 10, 0 ) );
			// // 	}
			// }

			// DebugOverlay.ScreenText( 10, $"{Agent.TargetEndPosition}", 0.01f );
			// // if ( Agent.HasDestination )
			// // {
			// // 	float dist = Agent.TargetEndPosition.Distance( Position );
			// 	// DebugOverlay.Text( Position, $"{dist}", 0.01f );
			// // 	if ( dist <= 100.0f )
			// // 	{
			// // 		DebugOverlay.ScreenText(10, $"Clear Destination {dist} | {Agent.TargetEndPosition}", 0.1f);
			// // 		ClearDestination();
			// // 	}
			// // }
		}

		[Event.Tick.Server]
		public void Tick()
		{
			using var _a = Sandbox.Debug.Profile.Scope( "NpcTest::Tick" );

			InputVelocity = 0;

			if ( Steer != null )
			{
				using var _b = Sandbox.Debug.Profile.Scope( "Steer" );

				Steer.Tick( Position );

				if ( !Steer.Output.Finished )
				{
					InputVelocity = Steer.Output.Direction.Normal;
					Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed );
				}

				if ( NavPath.nav_drawpath )
				{
					Steer.DebugDrawPath();
				}
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

			var animHelper = new CitizenAnimationHelper( this );

			LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
			animHelper.WithLookAt( EyePos + LookDir );
			animHelper.WithVelocity( Velocity );
			animHelper.WithWishVelocity( InputVelocity );
			
			Think();
		}

		protected virtual void Move( float timeDelta )
		{
			var bbox = BBox.FromHeightAndRadius( 64, 4 );
			//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

			MoveHelper move = new( Position, Velocity );
			move.MaxStandableAngle = 50;
			move.Trace = move.Trace.Ignore( this ).Size( bbox );

			if ( !Velocity.IsNearlyZero( 0.001f ) )
			{
			//	Sandbox.Debug.Draw.Once
			//						.WithColor( Color.Red )
			//						.IgnoreDepth()
			//						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

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
	}
} // namespace survivez.Entities

