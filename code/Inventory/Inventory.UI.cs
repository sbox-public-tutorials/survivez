using Sandbox;
using survivez.Controllers;
using System.Collections.Generic;

namespace survivez.Inventory
{
	public partial class Inventory : BaseInventory
	{
		public List<IItem> Items { get; set; }
		public SPlayer Player { get => (SPlayer)Owner; }

	}
}
