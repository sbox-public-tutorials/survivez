using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace survivez.HUD.Inventory
{
	// Inventory Item is the individual items in the hand or bag.
	// https://github.com/Facepunch/sandbox/blob/master/code/ui/InventoryIcon.cs
	public class InventoryItem : Panel
	{
		public Entity Item;
		public Panel NamePanel;
		public Label Name;
		public Panel NumberPanel;
		public Label Number;

		public InventoryItem( int i, Panel parent )
		{
			Parent = parent;
			SetClass( "inventory-item", true );
			NumberPanel = AddChild<Panel>();
			NumberPanel.SetClass( "slot-number-panel", true );
			Number = NumberPanel.AddChild<Label>();
			Number.SetText( $"{i}" );
			Number.SetClass( "slot-number", true );

			NamePanel = AddChild<Label>();
			NamePanel.SetClass( "item-name-panel", true );
			Name = NamePanel.AddChild<Label>();
			Name.SetText( "Item goes here" );
			Name.SetClass( "item-name", true );
		}

		public void Activate()
		{			
			SetClass( "active", true );
		}
		public void DeActivate()
		{
			RemoveClass( "active" );
		}

		public void Clear()
		{
			//Name.SetText( "" );
			SetClass( "active", false );
		}
	}
}
