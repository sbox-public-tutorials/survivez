using Sandbox;
using survivez.Controllers;


namespace survivez.Inventory.Items
{
	class Medkit: ConsumableItem
	{

		public override void Spawn()
		{
			ItemName = "Medkit";
			ItemWeight = 1f;
			base.Spawn();
		}

		public virtual void ItemDrop( SPlayer player, Vector3 position, Rotation direction )
		{
			SetModel( "models/citizen_props/cardboardbox01.vmdl" );
			Scale = 0.5f;
			base.ItemDrop( player, position, direction );
		}
	}
}
