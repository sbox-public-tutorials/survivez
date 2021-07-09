using Sandbox;
using System.Linq;
using survivez.Nav;
using survivez.Controllers;
using survivez.Misc;

namespace survivez.Entities
{
	// TODO :: Make a NPC Abstract Partial Class - That is the base for all NPC Entities (ie. Zombies).
	// TODO :: Cleanup NavMeshAgent Classes to make it easier to iterate upon.
	// TODO :: Make Zombies wander, until provoked or alerted of a player.
	// TODO :: Make Player Action Sounds (Sneak, Run, Walk, and Fire a weapon) emit sound levels.
	// TODO :: Make Player Smell (over time of being in an area, a player can build up a scent; and the zombie may get alerted).

	[Library( "sz_zombie_standard", Description = "Standard Zombie." )]
	public partial class Zombie : NPC
    {
		public Entity TargetEnemy { get; private set; }
		public float MinDamage { get; set; } = 5.0f;
		public float MaxDamage { get; set; } = 10.0f;
		public float SearchRadius { get; set; } = 1000f;

		public static Vector3 CenterPosition = new Vector3(0,0,50);

		public float AttackRadius { get; set; } = 60.0f;
		public float lastAttackTime;
		public float AttackDelay { get; set; } = 1.0f;
		public float RandomDeathTime { get; set; }

		public Zombie() : base()
		{
			Steer = new Follow( this, null );
			SetAnimInt( "holdtype", 4 );
			SetAnimInt( "holdtype_handedness", 0 );
			SetAnimFloat( "aimat_weight", 1.0f );
			RenderColor = Color.Green;
			var rnd = new System.Random();
			RandomDeathTime = ((float)rnd.NextDouble() * 5f) + 5f;
		}

		public void SetDestination(Vector3 _position)
		{
			Steer.Target = _position;
		}

		public override void TakeDamage( DamageInfo info )
		{
			lastAttackTime = -AttackDelay;
			base.TakeDamage( info );
		}

		public override void Think()
		{
			lastAttackTime += Time.Delta;

			var phase = SurviveZ.GetRoundPhase();
			if ( phase == Misc.RoundPhase.Preparing )
			{
				if ( RandomDeathTime == 0)
				{
					RandomDeathTime = 5f;
				}
				TakeDamage( new DamageInfo() { Damage = RandomDeathTime * Time.Delta, Flags = DamageFlags.Burn } );
			}
			if (this.Position.z < -500)
			{
				Suicide();
			}

			SetAnimInt( "holdtype_handedness", 0 );


			// Ensure that the enemy isn't too far from us.
			if (TargetEnemy != null && TargetEnemy.IsValid() )
			{
				float dist = TargetEnemy.Position.Distance( Position );
				if ( dist > (SearchRadius * 1.2f) )
				{
					
					SetAnimFloat( "holdtype_pose_hand", 0.06f );
					TargetEnemy = null;
					(Steer as Follow).FollowTarget = null;
				}
				else if ( dist <= AttackRadius )
				{
					Attack();
				}
			}

			// Search for nearby players.
			if ( TargetEnemy == null || !TargetEnemy.IsValid() )
			{
				TargetEnemy = FindNearestTarget();
			}
			// Follow player...
			if (TargetEnemy != null && TargetEnemy.IsValid())
			{
				(Steer as Follow).FollowTarget = TargetEnemy;
			} else
			{
				SetDestination( CenterPosition );
			}
		}

		public void Suicide()
		{
			TakeDamage( new DamageInfo() { Damage = Health + 1, Flags = DamageFlags.Burn } );
		}

		public void Attack()
		{
			float Damage = Rand.Float( MinDamage, MaxDamage );
			SetAnimFloat( "holdtype_pose_hand", 0f );
			if ( lastAttackTime > AttackDelay )
			{
				SetAnimInt( "holdtype_handedness", SRandom.Int(0,3) );
				SetAnimFloat( "holdtype_attack", SRandom.Float( 0, 8 ) );
				SetAnimBool( "b_attack", true );
				{
					TargetEnemy.TakeDamage( new DamageInfo() { Damage = Damage, Attacker = this, Flags = DamageFlags.Blunt } );
				}
				lastAttackTime = 0.0f;
			}
		}

		public Entity FindNearestTarget()
		{
			SPlayer[] snapshot = Entity.All.OfType<SPlayer>().ToArray();

			Entity target = null;
			float lastNearestTarget = float.MaxValue;

			foreach ( var player in snapshot )
			{
				if ( player == null || !player.IsValid() ) continue;

				float dist = player.Position.Distance( Position );
				if ( dist > SearchRadius ) continue;
				if ( dist > lastNearestTarget ) continue;
				target = player;
				lastNearestTarget = dist;
			}

			return target;
		}
	}
} // namespace survivez.Entities
