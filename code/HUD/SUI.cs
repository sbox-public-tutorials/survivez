using Sandbox;
using Sandbox.UI;
using survivez.Controllers;
using survivez.HUD.Crosshair;

namespace survivez.HUD
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class SUI : Sandbox.HudEntity<RootPanel>
	{
		public SUI()
		{
			if ( IsClient )
			{
				Log.Info( "SUI!" );
				if ( Local.Pawn is not SPlayer pawn )
					return;

				Log.Info( "SUI - Spawning everything!" );
				pawn.CrosshairCanvas = RootPanel.AddChild<SCrosshairCanvas>();
				pawn.CrosshairCanvas.SetCrosshair( new SCrosshair(), new SCrosshairPhysical() );
				RootPanel.AddChild<NameTags>();
				RootPanel.AddChild<ChatBox>();
				RootPanel.AddChild<VoiceList>();
				RootPanel.AddChild<KillFeed>();
				RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
				RootPanel.AddChild<Health>();
				RootPanel.AddChild<RoundPanel>();
			}
		}
	}

}
