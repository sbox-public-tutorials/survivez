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
				if ( Local.Pawn is not SPlayer pawn )
					return;

				pawn.CrosshairCanvas = RootPanel.AddChild<SCrosshairCanvas>();
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
