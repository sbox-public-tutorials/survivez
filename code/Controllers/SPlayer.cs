using Sandbox;
using survivez.HUD;
using survivez.HUD.Crosshair;
using survivez.Inventory;
using survivez.Weapons;

namespace survivez.Controllers
{
	partial class SPlayer : Player
	{
		public SCrosshairCanvas CrosshairCanvas { get; set; }

		public bool IsAlive { get => Health > 0; }

		private DamageInfo lastDamage;

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new SPlayerController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new SPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new SPlayerCamera();

			EnableAllCollisions			= true;
			EnableDrawing				= true;
			EnableHideInFirstPerson		= true;
			EnableShadowInFirstPerson	= true;

			Inventory = new PlayerInventory( this );
			Inventory.Add( new SMG(), true );
			Inventory.Add( new Axe() );
			Inventory.Add( new Shotgun() );

			SpawnHUD( To.Single(GetClientOwner()) );

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );
		}

		[ClientRpc]
		public void SpawnHUD()
		{
			Log.Info( "Reloading Client UI." );
			Local.Hud?.Delete();
			new SUI();
		}

		[ClientRpc]
		public void TookDamage( DamageFlags damageFlags, Vector3 forcePos, Vector3 force )
		{
		}

		public override void TakeDamage( DamageInfo info )
		{
			/*
			if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
			{
				info.Damage *= 10.0f;
			}
			*/
			Log.Info( $"Player Health: {Health} | {info.Damage}" );
			lastDamage = info;

			TookDamage( lastDamage.Flags, lastDamage.Position, lastDamage.Force );

			base.TakeDamage( info );
		}

		public override void OnKilled()
		{
			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
			Inventory.DeleteContents();
			Inventory = null;

			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
