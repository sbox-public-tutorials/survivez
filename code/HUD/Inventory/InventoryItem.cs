using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace survivez.HUD.Inventory
{
	// Inventory Item is the individual items in the hand or bag.
	// https://github.com/Facepunch/sandbox/blob/master/code/ui/InventoryIcon.cs
	public class InventoryItem : Panel
	{
		public Entity TargetEnt;
		public Label Label;
		public Label Number;

		public InventoryItem( int i, Panel parent )
		{
			Parent = parent;
			Label = Add.Label( "empty", "item-name" );
			Number = Add.Label( $"{i}", "slot-number" );
		}

		public void Activate()
		{
			SetClass( "active", TargetEnt.IsActiveChild() );
		}

		public void Clear()
		{
			Label.Text = "";
			SetClass( "active", false );
		}
	}
}
