using Sandbox;
using System;

namespace survivez.Controllers
{
	public class SPlayerAnimator : PawnAnimator
	{
		TimeSince TimeSinceFootShuffle = 60;

		float duck;

		public override void Simulate()
		{
			// TODO : Find the mouse position, and point to it.
			//Vector3 mouseOffset = Screen.GetDirection( Mouse.Position ); // Cannot do this, we are in the server.

			var idealRotation = Pawn.Rotation;//Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );
			//var idealRotation = Rotation.LookAt( mouseOffset.WithZ( 0 ), Vector3.Up );

			// Rotate the body in said rotation...
			DoRotation( idealRotation );
			DoWalk( idealRotation );

			//
			// Let the animation graph know some shit
			//
			bool noclip = HasTag( "noclip" );

			SetParam( "b_grounded", GroundEntity != null || noclip );
			SetParam( "b_noclip", noclip );
			SetParam( "b_swim", Pawn.WaterLevel.Fraction > 0.5f );

			Vector3 aimPos = Pawn.EyePos + Pawn.Rotation.Forward * 200;
			Vector3 lookPos = aimPos;

			//
			// Look in the direction what the player's input is facing
			//
			//SetLookAt( "lookat_pos", lookPos ); // old
			//SetLookAt( "aimat_pos", aimPos ); // old

			//SetLookAt( "aim_eyes", lookPos );
			//SetLookAt( "aim_head", lookPos );
			//SetLookAt( "aim_body", aimPos );

			SetParam( "b_ducked", HasTag( "ducked" ) ); // old

			if ( HasTag( "ducked" ) ) duck = duck.LerpTo( 1.0f, Time.Delta * 10.0f );
			else duck = duck.LerpTo( 0.0f, Time.Delta * 5.0f );

			SetParam( "duck", duck );

			if ( Pawn.ActiveChild is BaseCarriable carry )
			{
				carry.SimulateAnimator( this );
			}
			else
			{
				SetParam( "holdtype", 0 );
				SetParam( "aimat_weight", 0.5f ); // old
				SetParam( "aim_body_weight", 0.5f );
			}

		}

		public virtual void DoRotation( Rotation idealRotation )
		{
			//
			// Our ideal player model rotation is the way we're facing
			//
			var allowYawDiff = Pawn.ActiveChild == null ? 90 : 50;

			float turnSpeed = 0.01f;
			if ( HasTag( "ducked" ) ) turnSpeed = 0.1f;

			//
			// If we're moving, rotate to our ideal rotation
			//
			Rotation = Rotation.Slerp( Rotation, idealRotation, WishVelocity.Length * Time.Delta * turnSpeed );

			//
			// Clamp the foot rotation to within 120 degrees of the ideal rotation
			//
			Rotation = Rotation.Clamp( idealRotation, allowYawDiff, out var change );

			//
			// If we did restrict, and are standing still, add a foot shuffle
			//
			if ( change > 1 && WishVelocity.Length <= 1 ) TimeSinceFootShuffle = 0;

			SetParam( "b_shuffle", TimeSinceFootShuffle < 0.1 );
		}

		void DoWalk( Rotation idealRotation )
		{
			//
			// These tweak the animation speeds to something we feel is right,
			// so the foot speed matches the floor speed. Your art should probably
			// do this - but that ain't how we roll
			//
			SetParam( "walkspeed_scale", 2.0f / 190.0f );
			SetParam( "runspeed_scale", 2.0f / 320.0f );
			SetParam( "duckspeed_scale", 2.0f / 80.0f );

			//
			// Work out our movement relative to our body rotation
			//
			var moveDir = WishVelocity;
			var forward = idealRotation.Forward.Dot( moveDir );
			var sideward = idealRotation.Right.Dot( moveDir );


			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();
			//DebugOverlay.Text( Pos, $"{angle}" );
			SetParam( "move_direction", angle );


			//
			// Set our speeds on the animgraph
			//
			var speedScale = Pawn.Scale;

			SetParam( "forward", forward );
			SetParam( "sideward", sideward );
			SetParam( "wishspeed", speedScale > 0.0f ? WishVelocity.Length / speedScale : 0.0f );
		}

		public override void OnEvent( string name )
		{
			// DebugOverlay.Text( Pos + Vector3.Up * 100, name, 5.0f );

			if ( name == "jump" )
			{
				Trigger( "b_jump" );
			}

			base.OnEvent( name );
		}
	}
}
