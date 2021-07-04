using Sandbox;
using System.Linq;
using System;

namespace survivez.Entities
{
	// This NPC class builds the foundation for AI direction.
    public abstract partial class NPC : BaseNPC
    {
        private DamageInfo lastDamage;

		public static Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;

		Vector3 InputVelocity;
		Vector3 LookDir;

        public NPC()
		{
		}

		public override void Spawn()
		{
			base.Spawn();

			Init();
		}

		public virtual void Init() {}
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
        public virtual void OnDeath() {}
        public virtual void OnDamaged( DamageInfo info ) {}

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

