
using Sandbox;
using survivez.Controllers;
using survivez.Entities;
using survivez.Misc;
using System.Linq;

namespace survivez
{

	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client.
	///
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	///
	/// Your game needs to be registered (using [Library] here) with the same name
	/// as your game addon. If it isn't then we won't be able to find it.
	/// </summary>
	[Library( "survivez" )]
	public partial class SurviveZ : Game
	{
		public static TimerManager TimerManager;


		[Event.Hotload]
		public static void OnReload()
		{
			Initialize();
			if ( Host.IsServer )
			{
				SPlayer[] players = All.OfType<SPlayer>().ToArray();
				foreach ( SPlayer player in players )
				{
					player.Respawn();
				}
				if ( Game != null )
				{
					Game.RoundSystem = new WaveDefenseRoundSystem();
					Game.RoundSystem.OnRoundOrPhaseChange();
				}
			}
		}

		public static SurviveZ Game { get; set; }

		public WaveDefenseRoundSystem RoundSystem { get; set; }

		public SurviveZ()
		{
			Game = this;
			RoundSystem = new WaveDefenseRoundSystem();
			Initialize();
		}

		public static void Initialize()
		{
			// Initiates at the Client & Server.
			TimerManager = new TimerManager();
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new SPlayer();
			client.Pawn = player;

			player.Respawn();
			Game.RoundSystem.UpdatePlayersRoundInformation( player );
		}

		[Event.Tick.Server]
		public void ServerTick()
		{
			var delta = Time.Delta;
			RoundSystem.Tick( delta );

			if ( TimerManager != null )
			{
				TimerManager.Update();
			}
		}

		[Event.Tick.Client]
		public void ClientTick()
		{
			if ( TimerManager != null )
			{
				TimerManager.Update();
			}
		}

		public static bool CanSpawnZombie()
		{
			if ( Entity.All.OfType<Zombie>().ToArray().Length >= 40 )
			{
				return false;
			}
			return true;
		}

	}

}
