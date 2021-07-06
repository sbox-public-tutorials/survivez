using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace survivez.HUD.Inventory
{
	// Inventory Hand is the possible items that can be selected from 1-0 on the keyboard.
	// https://github.com/Facepunch/sandbox/blob/master/code/ui/InventoryBar.cs
	public class InventoryHand : Panel
	{
		private readonly List<InventoryItem> slots = new();

		public InventoryHand()
		{
			for ( int i = 0; i < 9; i++ )
			{
				var icon = new InventoryItem( i + 1, this );
				slots.Add( icon );
			}
			Log.Info( "Constructor - InventoryHand" );
		}

		public override void Tick()
		{
			var player = Local.Pawn;
			if ( player == null ) return;
			var inventory = player.Inventory;
			if ( inventory == null )
				return;

			Log.Info( "ProcessClientInput - InventoryHand" );

			if ( Input.Pressed( InputButton.Slot1 ) ) SetActiveSlot( inventory, 0 );
			if ( Input.Pressed( InputButton.Slot2 ) ) SetActiveSlot( inventory, 1 );
			if ( Input.Pressed( InputButton.Slot3 ) ) SetActiveSlot( inventory, 2 );
			if ( Input.Pressed( InputButton.Slot4 ) ) SetActiveSlot( inventory, 3 );
			if ( Input.Pressed( InputButton.Slot5 ) ) SetActiveSlot( inventory, 4 );
			if ( Input.Pressed( InputButton.Slot6 ) ) SetActiveSlot( inventory, 5 );
			if ( Input.Pressed( InputButton.Slot7 ) ) SetActiveSlot( inventory, 6 );
			if ( Input.Pressed( InputButton.Slot8 ) ) SetActiveSlot( inventory, 7 );
			if ( Input.Pressed( InputButton.Slot9 ) ) SetActiveSlot( inventory, 8 );

			for ( int i = 0; i < slots.Count; i++ )
			{
				UpdateIcon( player.Inventory.GetSlot( i ), slots[i], i );
			}
		}

		private static void SetActiveSlot( IBaseInventory inventory, int i )
		{
			var player = Local.Pawn;
			if ( player == null )
				return;

			var ent = inventory.GetSlot( i );
			if ( ent == null )
				return;

			if ( player.ActiveChild == ent )
				return;

			//input.ActiveChild = ent;
		}

		private static void UpdateIcon( Entity ent, InventoryItem inventoryIcon, int i )
		{
			if ( ent == null )
			{
				inventoryIcon.Clear();
				return;
			}

			inventoryIcon.TargetEnt = ent;
			inventoryIcon.Label.Text = ent.ClassInfo.Title;
			inventoryIcon.Activate();
		}

	}
}
