using Sandbox;
using survivez.Inventory;
using survivez.Weapons;

namespace survivez.Controllers
{
	partial class SPlayer : Player
	{
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Log.Info( $"Respawn {IsServer} {IsClient}" );
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
			Inventory.Add( new Shotgun() );

			base.Respawn();
		}

		[ServerCmd]
		public static void GiveWantedRotation( Vector3 lookAtPos )
		{
			Entity pawn = ConsoleSystem.Caller.Pawn;
			pawn.Rotation = Rotation.LookAt( (lookAtPos - pawn.Position).WithZ( 0 ).Normal );
			pawn.EyeRot = pawn.Rotation;
			//Log.Info( $"Gonna set rotation {pawn} {lookAtPos}" );
			// DebugOverlay.Line(pawn.Position, lookAtPos, Color.Red, 1.0f, false);
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

		public override void OnKilled()
		{
			Inventory = null;

			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
