using Sandbox;
namespace survivez.Misc
{

	public enum RoundPhase : byte
	{
		Preparing = 0,
		Defending = 1,
	}

	public partial class RoundSystem
	{
		public int CurrentPhase { get; set; }
		public int CurrentRound { get; set; }
		public int TotalPhases { get; set; }
		public float[] PhaseDuration { get; set; }
		public float DefaultPhaseDuration { get; set; } // Round time in seconds
		public float StartTime { get; set; } // Round time in seconds
		public float EndTime { get; set; } // Round time in seconds

		public RoundSystem()
		{
			Initialise();
		}

		public virtual void Initialise()
		{
			CurrentPhase = 0;
			CurrentRound = 1;
		}

		public float GetPhaseDuration( int id )
		{
			if ( id < 0 || id >= PhaseDuration.Length )
			{
				return DefaultPhaseDuration;
			}
			else
			{
				return PhaseDuration[id];
			}
		}

		public void NextPhase()
		{
			OnPhaseEnd( CurrentPhase );
			CurrentPhase = CurrentPhase + 1;
			if ( CurrentPhase >= TotalPhases )
			{
				NextRound();
			}
			else
			{
				var phaseDuration = GetPhaseDuration( CurrentPhase );				
				StartTime = Time.Now;
				EndTime = Time.Now + phaseDuration;
				OnPhaseStart( CurrentPhase );
				OnRoundOrPhaseChange();
			}
		}

		public virtual void TryPhaseEnd()
		{
			if ( IsPhaseEnd() )
			{
				NextPhase();
			}
		}

		public void NextRound()
		{
			OnRoundEnd( CurrentRound );
			CurrentRound = CurrentRound + 1;
			CurrentPhase = 0;
			OnRoundOrPhaseChange();
		}

		public virtual bool IsPhaseEnd()
		{
			var now = Time.Now;
			if ( now >= EndTime )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsPhase(RoundPhase phase)
		{
			return CurrentPhase == (int)phase;
		}

		public void Tick( float delta )
		{
			TryPhaseEnd();
		}


		public virtual void OnPhaseEnd( int phase ) { }
		public virtual void OnPhaseStart( int phase ) { }
		public virtual void OnRoundStart( int round ) { }
		public virtual void OnRoundEnd( int round ) { }
		public virtual void OnRoundOrPhaseChange() { }


	}

}
