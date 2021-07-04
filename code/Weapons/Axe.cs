using Sandbox;
using survivez.Inventory;

namespace survivez.Weapons
{
    public partial class Axe : Weapon
    {
        // public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
        public override float PrimaryRate => 1;
        public override float SecondaryRate => 1;
        public override float ReloadTime => 0.5f;

        public override void Spawn()
        {
            base.Spawn();
			Zoom = 1000.0f;

			SetModel( "Content/models/survivez/weapons/Axe/axe.vmdl" );
        }

        public override void AttackPrimary()
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            (Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

            //
            // Tell the clients to play the shoot effects
            //
            ShootEffects();

            //
            // Shoot the bullets
            //
            ShootBullets( 10, 0.1f, 10.0f, 9.0f, 3.0f );
        }

        public override void AttackSecondary()
        {
			return;
        }

        [ClientRpc]
        protected override void ShootEffects()
        {
            Host.AssertClient();

            ViewModelEntity?.SetAnimBool( "fire", true );

            if ( IsLocalPawn )
            {
                new Sandbox.ScreenShake.Perlin( 1.0f, 1.5f, 2.0f );
            }

            CrosshairPanel?.CreateEvent( "fire" );
        }

        public override void Reload()
        {
            return;
        }


        public override void OnReloadFinish()
        {
            IsReloading = false;

            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;
        }

        public override void SimulateAnimator( PawnAnimator anim )
        {
            anim.SetParam( "holdtype", 4 ); // TODO this is shit
            anim.SetParam( "aimat_weight", 1.0f );
        }
    }
}
