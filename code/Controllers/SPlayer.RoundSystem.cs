using Sandbox;

namespace survivez.Controllers
{
	public partial class SPlayer
	{
		public int CurrentPhase { get; set; }
		public int CurrentRound { get; set; }
		public float PhaseEndTime { get; set; }
		public float PhaseStartTime { get; set; }
		[ClientRpc]
		public void UpdateRoundState( int round, int phase, float EndTime, float ServerTime )
		{
			var now = Time.Now;
			Host.AssertClient();

			if ( CurrentPhase != phase )
			{
				NewPhase();
				PhaseStartTime = now;
				PhaseEndTime = now + (EndTime - ServerTime);
			}
			if ( CurrentRound != round )
			{
				NewRound();
			}
			CurrentPhase = phase;
			CurrentRound = round;
		}

		private void NewRound()
		{
			Host.AssertClient();
			Log.Info( $"New Round - " );
		}

		private void NewPhase()
		{
			Host.AssertClient();
			Log.Info( "New Phase!" );
		}

	}
}
