using Sandbox;
using survivez.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace survivez.Inventory
{
	public partial class PlayerInventory : BaseInventory
	{
		public List<IItem> Items { get; set; }
		public SPlayer Player { get; set; }

	}
}
