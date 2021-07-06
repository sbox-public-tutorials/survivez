using Sandbox;
using survivez.Controllers;

namespace survivez.Inventory
{
	public interface IItem : IUse
	{

		public string ItemName { get; set; }
		public float ItemWeight { get; set; }
		public void OnItemPickup( SPlayer player );
		public void ItemDrop( SPlayer player, Vector3 position, Rotation direction );
	}
}
