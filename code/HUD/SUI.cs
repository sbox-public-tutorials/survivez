using Sandbox.UI;

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
				RootPanel.AddChild<NameTags>();
				RootPanel.AddChild<CrosshairCanvas>();
				RootPanel.AddChild<ChatBox>();
				RootPanel.AddChild<VoiceList>();
				RootPanel.AddChild<KillFeed>();
				RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
				RootPanel.AddChild<Health>();
			}
		}
	}

}
