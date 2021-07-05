using Sandbox;
using Sandbox.UI;
using survivez.Controllers;
using survivez.Entities;

namespace survivez.HUD.Crosshair
{
	public partial class SCrosshairPhysical : Panel
	{
		public const float startingScale = 5;
		public float scale = startingScale;

		public Vector2 position = Vector2.One / 2.0f;

		int fireCounter;

		public SCrosshairPhysical()
		{
			StyleSheet.Load( "/Content/ui/crosshair/crosshair.scss" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not SPlayer pawn )
				return;

			// TraceResult tr = Trace.Ray( pawn.EyePos, pawn.EyePos + Screen.GetDirection( Mouse.Position ) * 100000.0f ).Ignore( pawn ).Run();
			// Vector3 mousePosition = tr.EndPos.WithZ(0);
			// float dist = tr.EndPos.Distance(pawn.Position);
			// DebugOverlay.ScreenText(1, $"{dist}");
			//if (dist <= 150.0f)
			// {
			// 	DebugOverlay.ScreenText(2, $"Overriding...");
			// 	mousePosition = pawn.Position + new Vector3( Mouse.Position.x / Screen.Width, Mouse.Position.y / Screen.Height );
			// }
			Vector2 trToScreen = new Vector2( Mouse.Position.x / Screen.Width, Mouse.Position.y / Screen.Height );

			position = trToScreen;
			float padding = 0.01f;
			position.x = MathX.Clamp( position.x, 0.0f + padding, 1.0f - padding );
			position.y = MathX.Clamp( position.y, 0.0f + padding, 1.0f - padding );

			Style.Width = 12 * scale;
			Style.Height = 12 * scale;

			Style.Top	= new Length()	{ Unit = LengthUnit.Percentage,		Value = (position.y * 100.0f) };
			Style.Left	= new Length()	{ Unit = LengthUnit.Percentage,		Value = (position.x * 100.0f) };

			Style.Dirty();

			scale = scale.LerpTo( startingScale, Time.Delta * 5 );
			SetClass( "fire", fireCounter > 0 );

			if ( fireCounter > 0 )
				fireCounter--;
		}

		[PanelEvent("fire")]
		public void FireEvent()
		{
			scale = 10;
			fireCounter += 2;
		}
	}
}
