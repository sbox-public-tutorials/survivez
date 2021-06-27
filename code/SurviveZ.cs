
using Sandbox;
using survivez.Controllers;
using survivez.HUD;
using survivez.Entities;

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
		public static void Initialize()
		{
			new SUI();
		}

		[Event.Hotload]
		public static void OnReload()
		{
			if ( Host.IsClient )
			{
				Log.Info( "Reloading Client UI." );
				Local.Hud?.Delete();
				Initialize();
			}
		}

		public SurviveZ()
		{
			if ( IsServer )
			{
				Initialize();
			}
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
		}

		[ServerCmd("spawn_zombie")]
		public static void SpawnZombie()
		{
			Zombie npc = new()
			{
				Position = new Vector3( -290.21f, -2426.63f, 0.03f )
			};
			npc.RenderColor = Color.Green;
		}
	}

}
