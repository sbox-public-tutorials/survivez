using Sandbox;
using survivez.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace survivez.Inventory.Items
{
	public partial class ConsumableItem : ModelEntity, IItem
	{
		[Net]
		public string ItemName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		[Net]
		public float ItemWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



		public override void Spawn()
		{
			base.Spawn();
		}



		public bool IsUsable( Entity user )
		{
			throw new NotImplementedException();
		}

		public virtual void ItemDrop( SPlayer player, Vector3 position, Rotation direction )
		{
			var accelleration = direction.Forward * 128;

			//Scale = 0;
			Position = position;
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );


			PhysicsBody.Velocity = accelleration;
			Log.Info( Position );
		}



		public void OnItemPickup( SPlayer player )
		{
			Scale = 0;
		}

		public bool OnUse( Entity user )
		{
			throw new NotImplementedException();
		}
	}
}
