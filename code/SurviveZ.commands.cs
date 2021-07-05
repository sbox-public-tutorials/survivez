
using Sandbox;
using survivez.Controllers;
using survivez.Entities;
using survivez.Misc;
using System.Linq;

namespace survivez
{
	public partial class SurviveZ : Game
	{
		
		[ServerCmd( "sz_next_phase" )]
		public static void NextPhase()
		{
			Game.RoundSystem.NextPhase();
		}
	}

}
