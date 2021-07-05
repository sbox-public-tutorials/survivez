using Sandbox;
using Sandbox.UI;
using survivez.Controllers;
using survivez.Entities;

namespace survivez.HUD.Crosshair
{
	public partial class SCrosshair : Panel
	{
		public Vector2 position = Vector2.One / 2.0f;

		public SCrosshair()
		{
			StyleSheet.Load( "/Content/ui/crosshair/crosshair.scss" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not SPlayer pawn )
				return;

			Vector2 trToScreen = new Vector2( Mouse.Position.x / Screen.Width, Mouse.Position.y / Screen.Height );

			position = trToScreen;
			float padding = 0.01f;
			position.x = MathX.Clamp( position.x, 0.0f + padding, 1.0f - padding );
			position.y = MathX.Clamp( position.y, 0.0f + padding, 1.0f - padding );

			Style.Top	= new Length()	{ Unit = LengthUnit.Percentage,		Value = position.y * 100.0f };
			Style.Left	= new Length()	{ Unit = LengthUnit.Percentage,		Value = position.x * 100.0f };

			Style.Dirty();
		}
	}
}
