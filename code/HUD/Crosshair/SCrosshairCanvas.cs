using Sandbox.UI;

namespace survivez.HUD.Crosshair
{
	public partial class SCrosshairCanvas : Panel
	{
		public static SCrosshairCanvas Singleton;

		public static Panel CurrentCrosshair { get; private set; }

		public SCrosshairCanvas()
		{
			Singleton = this;
			StyleSheet.Load( "/Content/ui/crosshair/crosshaircanvas.scss" );
			SetClass( "crosshaircanvas", true );
		}

		public static void SetCrosshair( Panel crosshairPanel )
		{
			if ( Singleton == null )
				return;

			Singleton.DeleteChildren();
			crosshairPanel.Parent = Singleton;
			CurrentCrosshair = crosshairPanel;
		}
	}
}
