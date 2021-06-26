
using Sandbox;
using survivez.Controllers;
using survivez.HUD;
using Sandbox.Nav;

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
		}

		public SurviveZ()
		{
			if ( IsServer )
			{
				// Create a HUD entity. This entity is globally networked
				// and when it is created clientside it creates the actual
				// UI panels. You don't have to create your HUD via an entity,
				// this just feels like a nice neat way to do it.
				_ = new SUI();

				// Push the limits...
				for (var i = 0; i < 50; i++)
				{
					NpcTest npc = new NpcTest()
					{
						Position = new Vector3(2144.10f + (i * 10.0f), -2256.31f, 4.03f),
						Steer = new Wander()
					};
				}

				Log.Info( "My Gamemode Has Created Serverside!" );
			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
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
	}

}
