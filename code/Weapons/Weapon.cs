using Sandbox;
using Sandbox.UI;
using survivez.Controllers;
using survivez.HUD.Crosshair;
using survivez.Inventory;
using survivez.Misc;
using System;

namespace survivez.Weapons
{
	public abstract partial class Weapon : BaseWeapon, IItem
	{
		public int CurrentClip { get; protected set; } = 0;
		public int MaxClipSize { get; protected set; } = 0;
		public int AmmoBag { get; protected set; } = 0;
		public bool UnlimitedAmmo { get; protected set; } = true; 

		public virtual float ReloadTime => 1.0f;

		public float Zoom = 1.0f;

		public PickupTrigger PickupTrigger { get; protected set; }

		[Net, Predicted]
		public TimeSince TimeSinceReload { get; set; }

		[Net, Predicted]
		public bool IsReloading { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceDeployed { get; set; }
		public string ItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public float ItemWeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public Panel CrosshairPhysicalPanel;

		public override void Spawn()
		{
			base.Spawn();

			PickupTrigger = new PickupTrigger
			{
				Parent = this,
				Position = Position,
				EnableTouch = true
			};

		}

		public override void ActiveStart( Entity ent )
		{
			base.ActiveStart( ent );

			TimeSinceDeployed = 0;
		}

		public override void Reload()
		{
			if ( IsReloading )
				return;

			TimeSinceReload = 0;
			IsReloading = true;

			(Owner as AnimEntity)?.SetAnimBool( "b_reload", true );

			StartReloadEffects();
		}

		public void PlayAnimation()
		{

		}

		public override void Simulate( Client owner )
		{
			if ( TimeSinceDeployed < 0.6f )
				return;

			if ( !IsReloading )
			{
				base.Simulate( owner );
			}

			if ( IsReloading && TimeSinceReload > ReloadTime )
			{
				OnReloadFinish();
			}
		}

		public virtual void OnReloadFinish()
		{
			if (!UnlimitedAmmo)
			{
				// (0 - MaxClipSize) 
				int grabAmmo = Math.Clamp( Math.Max( AmmoBag - (CurrentClip - MaxClipSize), 0), 0, MaxClipSize );
				CurrentClip = grabAmmo;
				AmmoBag -= grabAmmo;
				if (AmmoBag < 0)
					AmmoBag = 0;
			}
			IsReloading = false;
		}

		[ClientRpc]
		public virtual void StartReloadEffects()
		{
			ViewModelEntity?.SetAnimBool( "reload", true );

			// TODO - player third person model reload
		}

		public override void CreateViewModel()
		{
			Host.AssertClient();

			if ( string.IsNullOrEmpty( ViewModelPath ) )
				return;

			if ( ViewModelPath.Length > 0 )
			{
				ViewModelEntity = new ViewModel
				{
					Position = Position,
					Owner = Owner,
					EnableViewmodelRendering = true
				};

				ViewModelEntity.SetModel( ViewModelPath );
			}
		}

		public override bool CanPrimaryAttack()
		{
			return base.CanPrimaryAttack() && (UnlimitedAmmo || CurrentClip > 0);
		}

		public virtual bool OnUse( Entity user )
		{
			if ( Owner != null )
				return false;

			if ( !user.IsValid() )
				return false;

			user.StartTouch( this );

			return false;
		}

		public virtual bool IsUsable( Entity user )
		{
			if ( Owner != null ) return false;

			if ( user.Inventory is Inventory.Inventory inventory )
			{
				return inventory.CanAdd( this );
			}

			return true;
		}

		public void Remove()
		{
			PhysicsGroup?.Wake();
			Delete();
		}


		public override void CreateHudElements()
		{
			// This is the wrong assumption!
		}

		[ClientRpc]
		protected virtual void ShootEffects()
		{
			Host.AssertClient();

			Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

			if ( IsLocalPawn )
			{
				_ = new Sandbox.ScreenShake.Perlin();
			}

			ViewModelEntity?.SetAnimBool( "fire", true );

			if ( Local.Pawn is not SPlayer pawn )
				return;
			if ( pawn.CrosshairCanvas != null )
			{
				pawn.CrosshairCanvas.CurrentCrosshair?.CreateEvent( "fire" );
				pawn.CrosshairCanvas.CurrentCrosshairPhysical?.CreateEvent( "fire" );
			}
		}

		/// <summary>
		/// Shoot a single bullet
		/// </summary>
		public virtual void ShootBullet( Vector3 pos, Vector3 dir, float spread, float force, float damage, float bulletSize )
		{
			var forward = dir;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			//
			// ShootBullet is coded in a way where we can have bullets pass through shit
			// or bounce off shit, in which case it'll return multiple results
			//
			foreach ( var tr in TraceBullet( pos, pos + forward * 5000, bulletSize ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				//
				// We turn predictiuon off for this, so any exploding effects don't get culled etc
				//
				using ( Prediction.Off() )
				{
					if (CurrentClip > 0 || UnlimitedAmmo)
					{
						var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage )
							.UsingTraceResult( tr )
							.WithAttacker( Owner )
							.WithWeapon( this );

						tr.Entity.TakeDamage( damageInfo );

						if ( !UnlimitedAmmo )
							CurrentClip--;
					}
				}
			}
		}

		/// <summary>
		/// Shoot a single bullet from owners view point
		/// </summary>
		public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
		{
			ShootBullet( Owner.EyePos, Owner.Rotation.Forward, spread, force, damage, bulletSize );
		}

		/// <summary>
		/// Shoot a multiple bullets from owners view point
		/// </summary>
		public virtual void ShootBullets( int numBullets, float spread, float force, float damage, float bulletSize )
		{
			var pos = Owner.EyePos;
			var dir = Owner.Rotation.Forward;

			for ( int i = 0; i < numBullets; i++ )
			{
				ShootBullet( pos, dir, spread, force / numBullets, damage, bulletSize );
			}
		}

		public void OnItemPickup( SPlayer player )
		{
			throw new System.NotImplementedException();
		}

		public void ItemDrop( SPlayer player, Vector3 position, Rotation direction )
		{
			throw new System.NotImplementedException();
		}
	}
}
