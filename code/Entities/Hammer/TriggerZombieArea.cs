using Sandbox;
using Hammer;
using survivez.Controllers;
using survivez.Misc;

namespace survivez
{
	/// <summary>
	/// A trigger volume that damages entities that touch it.
	/// </summary>
	[Library( "sz_trigger_zombie_area", Description = "Player goes into this area and dies straight away." )]
	[Hammer.Solid]
	public partial class TriggerZombieArea : BaseTrigger
	{
		[Property( "damage", Title = "Damage" )]
		public float Damage { get; set; }

		[Property( Title = "Damage Per Second" )]
		public string RoundsDamage { get; set; }

		public string LastRoundsDamage { get; set; }

		public float[] RoundsData { get; set; }

		[Event.Tick.Server]
		protected virtual void DealDamagePerTick()
		{
			if ( !Enabled )
				return;

			if (LastRoundsDamage != RoundsDamage)
			{
				CalculateDamage();
			}

			var damage = 0f;

			RoundSystem roundSystem = SurviveZ.Game?.RoundSystem;
			if ( roundSystem != null && RoundsData != null )
			{

				var phase = (int)roundSystem.CurrentPhase;
				if ( phase >= 0 && phase < RoundsData.Length )
				{
					damage = RoundsData[phase];
				}
				damage = damage * Time.Delta;

				foreach ( var entity in TouchingEntities )
				{
					if ( !entity.IsValid() )
						continue;

					if ( entity is SPlayer player )
					{

						player.TakeDamage( new DamageInfo() { Damage = damage, Flags = DamageFlags.Radiation } );
					}
				}
			}
		}

		public void CalculateDamage()
		{
			LastRoundsDamage = RoundsDamage;
			var parts = RoundsDamage.Split( ";" );
			float[] floats = new float[parts.Length];
			for ( int i = 0; i < parts.Length; i++ )
			{
				floats[i] = float.Parse( parts[i] );
			}
			
			RoundsData = floats;

		}

		/// <summary>
		/// Sets the damage per second for this trigger_hurt
		/// </summary>
		[Input]
		protected void SetDamage( float damage )
		{
			Damage = damage;
		}
	}
}
