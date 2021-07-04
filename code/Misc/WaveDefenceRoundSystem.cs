using survivez.Controllers;
using System.Linq;

namespace survivez.Misc
{
	public partial class WaveDefenseRoundSystem : RoundSystem
	{

		public override void Initialise()
		{
			base.Initialise();
			// TODO Replace with config
			DefaultPhaseDuration = 1000 * 60 * 5.0f;
			TotalPhases = 2;

			// float[] phaseDurations = { 1000.0f * 60.0f * 5.0f, 1000.0f * 60.0f * 10.0f };
			float[] phaseDurations = { 60f, 120f };
			PhaseDuration = phaseDurations;
		}


		public override void OnRoundOrPhaseChange()
		{
			SPlayer[] players = SurviveZ.All.OfType<SPlayer>().ToArray();

			foreach ( var player in players )
			{
				UpdatePlayersRoundInformation( player );
			}
		}

		public void UpdatePlayersRoundInformation( SPlayer player )
		{
			var now = Sandbox.Time.Now;
			player.UpdateRoundState( CurrentRound, CurrentPhase, EndTime, now );
		}
	}

}
