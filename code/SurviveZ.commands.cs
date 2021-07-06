using Sandbox;
using survivez.Entities;

namespace survivez
{
	public partial class SurviveZ : Game
	{
		[ServerCmd( "sz_next_phase" )]
		public static void NextPhase()
		{
			Game.RoundSystem.NextPhase();
		}

		[ServerCmd( "sz_add_spawner" )]
		public static void SpawnZombie()
		{
			var owner = ConsoleSystem.Caller.Pawn;

			if ( owner == null )
				return;

			ZombieSpawnerHammer spawner = new()
			{
				Position = owner.EyePos
			};
			spawner.ZombieTypes = "sz_zombie_standard;";
		}

		public static Entity InternalSpawnEntity( string entName )
		{
			var attribute = Library.GetAttribute( entName );

			if ( attribute == null )
			{
				Log.Error( $"{entName}" );
				return null;
			}

			/*
			if ( !attribute.Spawnable )
			{
				Log.Info( "Not Spawnable!" );
				return null;
			}
			*/

			var ent = Library.Create<Entity>( entName );
			Log.Info( $"Spawned Entity {ent}!" );
			
			return ent;
		}

		[ServerCmd( "sz_ent_spawn" )]
		public static void SpawnEntity( string entName )
		{
			var owner = ConsoleSystem.Caller.Pawn;

			if ( owner == null )
			{
				return;
			}

			var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 200 )
				.UseHitboxes()
				.Ignore( owner )
				.Size( 2 )
				.Run();

			Entity ent = InternalSpawnEntity( entName );
			if ( ent == null )
			{
				Log.Error( $"Failed to run `sz_spawn_entity` - '{entName}' doesn't exist!" );
				return;
			}
			if ( !ent.IsValid() )
			{
				Log.Error( $"Failed to run `sz_spawn_entity` - '{entName}' isn't valid!" );
				return;
			}
			if ( ent is BaseCarriable && owner.Inventory != null )
			{
				if ( owner.Inventory.Add( ent, true ) )
					return;
			}

			ent.Position = tr.EndPos;
			ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRot.Angles().yaw, 0 ) );

		}
		[ServerCmd( "sz_ent_list" )]
		public static void EntityList( )
		{
			var atts = Library.GetAll();
			foreach (var att in atts)
			{
				Log.Info( "Attribute Library: " + att.Title );
			}
		}

	}

}
