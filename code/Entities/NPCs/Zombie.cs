using Sandbox;
using System.Linq;
using System;
using survivez.Nav;
using survivez.Controllers;

namespace survivez.Entities
{
	// TODO :: Make a NPC Abstract Partial Class - That is the base for all NPC Entities (ie. Zombies).
	// TODO :: Cleanup NavMeshAgent Classes to make it easier to iterate upon.
	// TODO :: Make Zombies wander, until provoked or alerted of a player.
	// TODO :: Make Player Action Sounds (Sneak, Run, Walk, and Fire a weapon) emit sound levels.
	// TODO :: Make Player Smell (over time of being in an area, a player can build up a scent; and the zombie may get alerted).

    public partial class Zombie : NPC
    {
		public Entity TargetEnemy { get; private set; }
		public float MinDamage { get; set; } = 2.0f;
		public float MaxDamage { get; set; } = 8.0f;
		public float SearchRadius { get; set; } = 800;

		public Zombie() : base()
		{
			Steer = new Follow( this, null );
			this.SetAnimInt( "holdtype", 4 );
			this.SetAnimFloat( "aimat_weight", 1.0f );
		}

		public void SetDestination(Vector3 _position)
		{
			Steer.Target = _position;
		}

		public override void Think()
		{
			// Ensure that the enemy isn't too far from us.
			if (TargetEnemy != null)
			{
				float dist = TargetEnemy.Position.Distance( Position );
				if ( dist > (SearchRadius * 1.2f) )
				{
					TargetEnemy = null;
					(Steer as Follow).FollowTarget = null;
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
			}
		}

		public void Attack()
		{
			float Damage = Rand.Float( MinDamage, MaxDamage );
			this.SetAnimBool( "b_attack", true );
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
				//Log.Info($"Zombie Scanning : {player} | {dist}");
				target = player;
				lastNearestTarget = dist;
			}

			return target;
		}
	}
} // namespace survivez.Entities
