using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace survivez.HUD.Inventory
{

	// Inventory Hand is the possible items that can be selected from 1-0 on the keyboard.
	// https://github.com/Facepunch/sandbox/blob/master/code/ui/InventoryBar.cs
	public class InventoryHand : Panel
	{
		private readonly List<InventoryItem> Slots = new();

		public int CurrentSlot { get; set; }


		const float INVENTORY_ITEM_COUNT = 4;

		public InventoryHand()
		{

			StyleSheet.Load( "/Content/ui/inventory/base.scss" );

			SetClass( "inventory-hand-container", true );

			Slots = new List<InventoryItem>();
			CurrentSlot = -1;

			for ( int i = 0; i < INVENTORY_ITEM_COUNT; i++ )
			{
				var icon = new InventoryItem( i + 1, this );
				AddChild( icon );
				Slots.Add( icon );
			}

			SetClientActiveSlot( 0 );

			Log.Info( "Constructor - InventoryHand" );
		}

		public override void Tick()
		{
			if ( Input.Pressed( InputButton.Slot1 ) ) SetClientActiveSlot(  0 );
			if ( Input.Pressed( InputButton.Slot2 ) ) SetClientActiveSlot(  1 );
			if ( Input.Pressed( InputButton.Slot3 ) ) SetClientActiveSlot(  2 );
			if ( Input.Pressed( InputButton.Slot4 ) ) SetClientActiveSlot(  3 );
			if ( Input.Pressed( InputButton.Slot5 ) ) SetClientActiveSlot(  4 );
			if ( Input.Pressed( InputButton.Slot6 ) ) SetClientActiveSlot(  5 );
			if ( Input.Pressed( InputButton.Slot7 ) ) SetClientActiveSlot(  6 );
			if ( Input.Pressed( InputButton.Slot8 ) ) SetClientActiveSlot(  7 );
			if ( Input.Pressed( InputButton.Slot9 ) ) SetClientActiveSlot(  8 );
		}

		public IBaseInventory GetInventory()
		{

			var player = Local.Pawn;
			if ( player == null )
			{
				return null;
			}

			return player?.Inventory;
		}

		public Entity GetItemFromInventory(int i)
		{
			var inventory = GetInventory();
			if (inventory != null)
			{
				return inventory.GetSlot( i );
			}
			return null;
		}

		[ServerCmd]
		public static void SetServerActiveSlot(int i)
		{
			var player = ConsoleSystem.Caller?.Pawn;
			var inventory = player?.Inventory;
			if (inventory != null)
			{				
				inventory.SetActiveSlot( i,true );
			}
		}

		public void SetClientActiveSlot(int i)
		{
			Host.AssertClient();
			if ( i != CurrentSlot )
			{				
				CurrentSlot = i;
				SetServerActiveSlot( i );
				if ( i >= 0 && i < INVENTORY_ITEM_COUNT )
				{
					for ( int slotIndex = 0; slotIndex < Slots.Count; slotIndex++ )
					{
						var slot = Slots[slotIndex];
						if ( slotIndex == i )
						{
							slot.Activate();
						}
						else
						{
							slot.DeActivate();
						}
					}
				}
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

		public void UpdateIcons()
		{
			var player = Local.Pawn;
			if ( player == null ) return;
			var inventory = player.Inventory;
			if ( inventory == null )
				return;

			for ( int i = 0; i < Slots.Count; i++ )
			{
				UpdateIcon( player.Inventory.GetSlot( i ), Slots[i], i );
			}
		}
		private static void UpdateIcon( Entity ent, InventoryItem inventoryIcon, int i )
		{
			if ( ent == null )
			{
				inventoryIcon.Clear();
				return;
			}

			//inventoryIcon.Item = ent;
			//inventoryIcon.Name.Text = ent.ClassInfo.Title;
			inventoryIcon.Activate();
		}

	}
}
