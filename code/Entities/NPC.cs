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

        public NPC()
		{
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

