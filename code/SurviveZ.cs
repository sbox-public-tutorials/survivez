
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
		[Event.Hotload]
		public static void OnReload()
		{
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
		}

		public static bool CanSpawnZombie()
		{
			if ( Entity.All.OfType<Zombie>().ToArray().Length >= 40 )
			{
				return false;
			}
			return true;
		}

		[ServerCmd( "spawn_zombie" )]
		public static void SpawnZombie()
		{
			ZombieSpawner spawner = new()
			{
				Position = new Vector3( -290.21f, -2426.63f, 0.03f )
			};
		}
	}

}
