using Sandbox;
using survivez.Misc;

namespace survivez.Entities
{
	[Library( "sz_itemspawner", Description = "Spawns a random item in it's list." )]
	[Hammer.EditorModel( "models/citizen_props/cardboardbox01.vmdl" )]
	[Hammer.EntityTool( "Random Item Spawner", "SurviveZ", "Defines where Items will spawn." )]
	public partial class RandomItemSpawnerHammer : Entity
	{
		[Property( Title = "Item Attributes" )]
		public string ItemTypes { get; set; }
		[Property( Title = "Spawn Radius" )]
		public float SpawnRadius { get; set; } = 200.0f;
		[Property( Title = "Spawn Rate" )]
		public float SpawnRate { get; set; } = 2;

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

			string[] types = ItemTypes.Split( ";" );

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
