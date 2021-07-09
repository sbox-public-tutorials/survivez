using Sandbox;
using survivez.Misc;

namespace survivez.Entities
{
	[Library( "sz_resourcecluster", Description = "Spawns a random Resource." )]
	[Hammer.EditorModel( "models/rust_props/rubble_piles/rubble_pile_3x3_junkyard.vmdl" )]
	[Hammer.EntityTool( "Random Resource Spawner", "SurviveZ", "Defines where Resource will spawn." )]
	public partial class ResourceClusterHammer : Entity
	{
		[Property( Title = "Resource Attributes" )]
		public string ResourceTypes { get; set; }
		[Property( Title = "Spawn Radius" )]
		public float SpawnRadius { get; set; } = 600.0f;
		[Property( Title = "Spawn Rates" )]
		public float SpawnRate { get; set; } = 3;

		private const float spawnDelay = 1.0f;
		private float lastSpawn = 0.0f;

		[Event.Tick.Server]
		public void Tick()
		{
			RoundSystem roundSystem = SurviveZ.Game.RoundSystem;
			if ( roundSystem.CurrentPhase == (int)RoundPhase.Preparing )
			{
				lastSpawn += Time.Delta;
				if ( lastSpawn > spawnDelay )
				{
					for ( var i = 0; i < SpawnRate; i++ )
					{
						SpawnResource();
					}
					lastSpawn = 0.0f;
				}
			}
		}

		public Entity SpawnResource()
		{
			Vector3 spawnPosition = this.Position + new Vector3( Rand.Float( -SpawnRadius, SpawnRadius ), Rand.Float( -SpawnRadius, SpawnRadius ), 0 );

			if ( !SurviveZ.CanSpawnZombie() )
				return null;

			string[] types = ResourceTypes.Split( ";" );

			if ( types.Length == 0 )
				return null;

			Entity ent = SurviveZ.InternalSpawnEntity( types[Rand.Int(0, types.Length - 1)]);
			if ( ent == null || !ent.IsValid() )
				return null;
			ent.Position = spawnPosition;

			return ent;
		}
	}
}
