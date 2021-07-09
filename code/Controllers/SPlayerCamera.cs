using Sandbox;

namespace survivez.Controllers
{
	class SPlayerCamera : Camera
	{
		private float zoomSpeed = 5f;
		private float zoom = 0.0f;
		private const float zoomMin = -15.0f;
		private const float zoomMax = 120.0f;

		private const float lerpSpeed = 4.0f;

		private const float cameraHeight = 350.0f;

		private Vector3 offset;

		public override void Activated()
		{
			if ( Local.Pawn is not SPlayer pawn )
				return;

			base.Activated();

			FieldOfView = 90;
			Viewer = null;

			offset = (Vector3.Forward * -10.0f);
			Pos = pawn.Position + offset;
		}

		[ServerCmd]
		public static void GiveWantedRotation( Vector3 lookAtPos )
		{
			Entity pawn = ConsoleSystem.Caller.Pawn;
			pawn.Rotation = Rotation.LookAt( (lookAtPos - pawn.Position).WithZ( 0 ).Normal );
			pawn.EyeRot = pawn.Rotation;
		}

		public override void Update()
		{
			if ( Local.Pawn is not SPlayer pawn )
				return;

			HandleZoom();

			LookTowards( pawn );

			ControlCamera( pawn );
		}

		public void HandleZoom()
		{
			zoomSpeed = 5f;
			zoom -= Input.MouseWheel * zoomSpeed;
			zoom = MathX.Clamp( zoom, zoomMin, zoomMax );
		}

		public void ControlCamera( SPlayer pawn )
		{
			float distance = (cameraHeight + zoom) * pawn.Scale;

			Vector3 mouseOffset = Screen.GetDirection( Mouse.Position ).WithZ(0);

			Vector3 mouseToLocal = mouseOffset * 200.0f;

			Vector3 lookOffset = mouseToLocal;


			Vector3 niceFeelOffset = (Vector3.Up * distance) + (Vector3.Forward * -distance/2.2f);
			Vector3 center = pawn.Position + niceFeelOffset;

			float weaponZoom = 1.0f;
			if ( Input.Down( InputButton.Attack2 ) )
			{
				/*
				Entity activeSlot = pawn.Inventory.Active;
				if ( activeSlot != null && activeSlot.IsValid() )
				{
					if ( activeSlot is Weapons.Weapon weapon)
					{
						weaponZoom = weapon.Zoom;
					}
					DebugOverlay.ScreenText( 1, $"Weapon Zoom : {weaponZoom} | {activeSlot}" );
				}
				*/
				center += lookOffset * weaponZoom;
			}

			if ( Pos.IsNaN ) // Fixed black screen of death issue...
			{
				Pos = center;
			}

			// Move the Camera into position.
			Pos = Vector3.Lerp( Pos, center + (pawn.Velocity / 3), Time.Delta * lerpSpeed );

			Rotation lockedRot = Rotation.FromPitch( 65.0f );
			Rot = lockedRot;
		}

		public void LookTowards( SPlayer pawn )
		{
			TraceResult tr = Trace.Ray( Input.Position, Input.Position + Screen.GetDirection( Mouse.Position ) * 100000.0f ).Ignore( pawn ).Run();

			Vector3 mouseToWorld = tr.EndPos.WithZ( 0 );

			Rotation lookRot = Rotation.LookAt( (mouseToWorld - pawn.Position).WithZ( 0 ).Normal );

			// Handle Client-side.
			pawn.Rotation = lookRot;
			pawn.EyeRot = pawn.Rotation;

			// Do not network, if we are dead.
			if ( pawn.IsAlive )
			{
				using ( Prediction.Off() )
				{
					// Handle Client-Side to Server-Side without prediction.
					GiveWantedRotation( mouseToWorld );
				}
			}
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
		}
	}
}
