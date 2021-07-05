using Sandbox.UI;

namespace survivez.HUD.Crosshair
{
	public partial class SCrosshairCanvas : Panel
	{
		public static Panel CurrentCrosshair { get; private set; }
		public static Panel CurrentCrosshairPhysical { get; private set; }

		public SCrosshairCanvas()
		{
			StyleSheet.Load( "/Content/ui/crosshair/crosshair.scss" );
		}

		// Allows you to change the Crosshair.
		// 		- This is Instantiated by Weapon.cs (CreateHudElements)
		public void SetCrosshair( Panel crosshairPanel, Panel crosshairPhysicalPanel = null )
		{
			this.DeleteChildren();
			if (crosshairPanel != null)
			{
				crosshairPanel.Parent = this;
			}
			if (crosshairPhysicalPanel != null)
			{
				crosshairPhysicalPanel.Parent = this;
			}
			CurrentCrosshair = crosshairPanel;
			CurrentCrosshairPhysical = crosshairPhysicalPanel;
		}
	}
}
