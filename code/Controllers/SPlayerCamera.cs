using Sandbox;

namespace survivez.Controllers
{
	class SPlayerCamera : Camera
	{
		public Vector3 offset { get; set; }

		private float zoom = 0.0f;
		private const float zoomMin = -15.0f;
		private const float zoomMax = 120.0f;

		private const float lerpSpeed = 4.0f;

		private const float nextUpdate = 0.03f;
		private float lastUpdate;

		// private const float MinDistanceView = 0.25f;
		// private const float MaxDistanceView = 0.75f;

		// Vector3 lastGoodMouseOffset = Vector3.Forward * MinDistanceView;


		[ServerCmd]
		public static void GiveWantedRotation( Vector3 lookAtPos )
		{
			Entity pawn = ConsoleSystem.Caller.Pawn;
			pawn.Rotation = Rotation.LookAt( (lookAtPos - pawn.Position).WithZ( 0 ).Normal );
			pawn.EyeRot = pawn.Rotation;
			// DebugOverlay.Line(pawn.Position, lookAtPos, Color.Red, 1.0f, false);
		}

		public override void Update()
		{
			if ( Local.Pawn is not SPlayer pawn )
				return;


			zoom -= Input.MouseWheel;
			zoom = MathX.Clamp( zoom, zoomMin, zoomMax );

			float distance = (250.0f + zoom) * pawn.Scale;

			Vector3 mouseOffset = Screen.GetDirection( Mouse.Position ).WithZ(0.0f);
			// Limiting the Mouse Offset.
			// float distFromCharacter = mouseOffset.Distance(Vector3.Zero);

			// if (distFromCharacter < MinDistanceView || distFromCharacter > MaxDistanceView)
			// {
			// 	mouseOffset = lastGoodMouseOffset;
			// }
			// else
			// {
			// 	lastGoodMouseOffset = mouseOffset;
			// }

			// DebugOverlay.ScreenText(12, $"Mouse Dist : {distFromCharacter}");

			Vector3 mouseToLocal = (mouseOffset * 200.0f);

			// DebugOverlay.ScreenText(11, $"[Good] Mouse Offset : {lastGoodMouseOffset}");
			// DebugOverlay.ScreenText(10, $"Mouse Offset : {mouseOffset}");

			//var lookOffset = (pawn.Rotation.Forward * (50.0f + zoom));
			Vector3 lookOffset = mouseToLocal;
			// DebugOverlay.ScreenText(11, $"Look Offset : {lookOffset}");

			Vector3 niceFeelOffset = (Vector3.Up * distance);
			Vector3 center = pawn.Position + offset + niceFeelOffset + (pawn.Velocity/3) + (lookOffset/2);
			if ( Pos.IsNaN ) // Fixed black screen of death issue...
			{
				Pos = center;
			}
			lastUpdate += Time.Delta;
			
			Vector3 lookAtPos = pawn.Position + (pawn.Velocity / 3) + lookOffset;
			Rotation lookRot = Rotation.LookAt( (lookAtPos - pawn.Position).WithZ( 0 ).Normal );
			
			//DebugOverlay.Line( pawn.Position, pawn.Position + (pawn.Rotation.Forward * 100.0f), Color.Blue, 1f, false ); // Server...
			pawn.Rotation = lookRot;
			pawn.EyeRot = pawn.Rotation;
			//DebugOverlay.Line( pawn.Position, pawn.Position + (pawn.Rotation.Forward * 100.0f), Color.Yellow, 1f, false ); // Client.

			if (pawn.IsAlive)
			{
				//if (nextUpdate < lastUpdate)
				{
					using ( Prediction.Off() )
					{
						GiveWantedRotation( lookAtPos );
					}
					lastUpdate = 0.0f;
				}
			}
			Pos = Vector3.Lerp( Pos, center, Time.Delta * lerpSpeed );
			Rotation lockedRot = Rotation.FromPitch(85.0f);
			Rot = lockedRot;
			// DebugOverlay.ScreenText(12, $"Camera Transform : {Pos} | {Rot}");

			FieldOfView = 90;

			Viewer = null;
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
		}
	}
}
