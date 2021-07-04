using Sandbox;
using survivez.Misc;
using System.Linq;

namespace survivez.Entities
{
	public class ZombieSpawner : Entity
	{
		public float spawnDelay = 1.0f;
		public float spawnRadius = 800.0f;

		public int spawnRate = 2;

		private float lastSpawn = 0.0f;

		[Event.Tick.Server]
		public void Tick()
		{
			RoundSystem roundSystem = SurviveZ.Game.RoundSystem;
			if ( roundSystem.CurrentPhase == (int)RoundPhase.Defending )
			{
				lastSpawn += Time.Delta;
				if ( lastSpawn > spawnDelay )
				{
					for ( var i = 0; i < spawnRate; i++ )
					{
						SpawnZombie();
					}
					lastSpawn = 0.0f;
				}
			}
		}

		public Zombie SpawnZombie()
		{
			Vector3 spawnPosition = this.Position + new Vector3( Rand.Float( -spawnRadius, spawnRadius ), Rand.Float( -spawnRadius, spawnRadius ), 0 );
			
			if ( !SurviveZ.CanSpawnZombie() )
				return null;

			Zombie npc = new()
			{
				Position = spawnPosition
			};

			return npc;
		}
	}
}
