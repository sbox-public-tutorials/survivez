using Sandbox.UI;

namespace survivez.HUD.Crosshair
{
	public partial class SCrosshairCanvas : Panel
	{
		public static SCrosshairCanvas Singleton;

		public static Panel CurrentCrosshair { get; private set; }
		public static Panel CurrentCrosshairPhysical { get; private set; }

		public SCrosshairCanvas()
		{
			Singleton = this;
			StyleSheet.Load( "/Content/ui/crosshair/crosshair.scss" );
		}

		// Allows you to change the Crosshair.
		// 		- This is Instantiated by Weapon.cs (CreateHudElements)
		public static void SetCrosshair( Panel crosshairPanel, Panel crosshairPhysicalPanel = null )
		{
			if ( Singleton == null )
				return;

			Singleton.DeleteChildren();
			if (crosshairPanel != null)
			{
				crosshairPanel.Parent = Singleton;
			}
			if (crosshairPhysicalPanel != null)
			{
				crosshairPhysicalPanel.Parent = Singleton;
			}
			CurrentCrosshair = crosshairPanel;
			CurrentCrosshairPhysical = crosshairPhysicalPanel;
		}
	}
}
