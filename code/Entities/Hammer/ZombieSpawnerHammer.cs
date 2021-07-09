using Sandbox;
using survivez.Misc;

namespace survivez.Entities
{
	[Library( "sz_zombiespawner", Description = "Spawns a random item in it's list." )]
	[Hammer.EditorModel( "models/editor/playerstart.vmdl" )]
	[Hammer.EntityTool( "Zombie Point", "SurviveZ", "Defines where zombies will spawn." )]
	public partial class ZombieSpawnerHammer : Entity
	{
		[Property( Title = "Zombie Attributes" )]
		public string ZombieTypes { get; set; }
		[Property( Title = "Spawn Radius" )]
		public float SpawnRadius { get; set; } = 600.0f;
		[Property( Title = "Spawn Rate" )]
		public float SpawnRate { get; set; } = 3;

		private const float spawnDelay = 1.0f;
		private float lastSpawn = 0.0f;

		[Event.Tick.Server]
		public void Tick()
		{
			RoundSystem roundSystem = SurviveZ.Game.RoundSystem;
			if ( roundSystem.CurrentPhase == (int)RoundPhase.Defending )
			{
				lastSpawn += Time.Delta;
				var spawnDelay = GetSpawnDelay();
				if ( lastSpawn > spawnDelay)
				{
					var spawnRate = GetSpawnRate();
					for ( var i = 0; i < spawnRate; i++ )
					{
						SpawnResource();
					}
					lastSpawn = 0.0f;
				}
			}
		}

		public float GetSpawnDelay()
		{
			return spawnDelay / SurviveZ.GetCurrentDifficulty().Clamp(0,10);
		}

		public float GetSpawnRate()
		{
			return SpawnRate;
		}

		public Entity SpawnResource()
		{
			Vector3 spawnPosition = this.Position + new Vector3( Rand.Float( -SpawnRadius, SpawnRadius ), Rand.Float( -SpawnRadius, SpawnRadius ), 0 );

			if ( !SurviveZ.CanSpawnZombie() )
			{
				return null;
			}
				

			string[] types = ZombieTypes.Split( ";" );

			if ( types.Length == 0 )
				return null;

			Entity ent = SurviveZ.InternalSpawnEntity( types[Rand.Int( 0, types.Length - 1 )] );
			if ( ent == null || !ent.IsValid() )
				return null;
			ent.Position = spawnPosition;

			return ent;
		}
	}
}
