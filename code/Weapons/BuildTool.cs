using Sandbox;

namespace survivez.Weapons
{
    public partial class BuildTool : Weapon
    {
        // public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
        public override float PrimaryRate => 1;
        public override float SecondaryRate => 1;
        public override float ReloadTime => 0.5f;

        public override void Spawn()
        {
            base.Spawn();
			Zoom = 1000.0f;

			SetModel( "models/citizen_props/crowbar01.vmdl" );
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
            ShootBullets( 1, 0f, 100f, 100.0f, 100f );
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

			base.ShootEffects();
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
            anim.SetParam( "holdtype", 2 ); // TODO this is shit
            anim.SetParam( "aimat_weight", 1.0f );
        }
    }
}
