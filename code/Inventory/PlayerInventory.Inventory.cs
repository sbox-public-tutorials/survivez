using Sandbox;
using survivez.Controllers;
using System.Collections.Generic;

namespace survivez.Inventory
{
	public partial class PlayerInventory : BaseInventory
	{
		public List<IItem> Items { get; set; }
		public SPlayer Player { get => (SPlayer)Owner; }

	}
}
