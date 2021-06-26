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

		public override void Update()
		{
			if ( Local.Pawn is not AnimEntity pawn )
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
			var lookOffset = mouseToLocal;
			// DebugOverlay.ScreenText(11, $"Look Offset : {lookOffset}");

			var niceFeelOffset = (Vector3.Up * distance);
			var center = pawn.Position + offset + niceFeelOffset + (pawn.Velocity/3) + lookOffset;
			if ( Pos.IsNaN )
			{
				Pos = center;
			}
			lastUpdate += Time.Delta;
			if (nextUpdate < lastUpdate)
			{
				SPlayer.GiveWantedRotation( pawn.Position + (pawn.Velocity/3) + lookOffset );
				lastUpdate = 0.0f;
			}
			// DebugOverlay.Line( pawn.Position, pawn.Position + lookOffset.WithZ(0), 1f, false );

			Pos = Vector3.Lerp( Pos, center, Time.Delta * lerpSpeed );
			Rotation lockedRot = Rotation.FromPitch(85.0f);
			Rot = lockedRot;
			DebugOverlay.ScreenText(12, $"Camera Transform : {Pos} | {Rot}");

			FieldOfView = 90;

			Viewer = null;
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
		}
	}
}
