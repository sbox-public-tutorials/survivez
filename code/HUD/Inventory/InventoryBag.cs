using Sandbox.UI;

namespace survivez.HUD.Inventory
{
	// Inventory Bag is the full inventory for the player...
	public class InventoryBag : Panel
	{
		public InventoryItem[,] InventoryItems { get; private set; }

		public void Init( int size )
		{
			InventoryItems = new InventoryItem[size, size];
		}
	}
}
