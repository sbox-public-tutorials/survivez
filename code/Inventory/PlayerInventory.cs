﻿using Sandbox;
using System;
using System.Linq;

namespace survivez.Inventory
{
	public partial class PlayerInventory : BaseInventory
	{
		public PlayerInventory( Player player ) : base( player )
		{
		}

		public void ClearInventory()
		{
			DeleteContents();
		}

		public override bool CanAdd( Entity entity )
		{
			if ( !entity.IsValid() )
				return false;

			if ( !base.CanAdd( entity ) )
				return false;

			return !IsCarryingType( entity.GetType() );
		}

		public override bool Add( Entity entity, bool makeActive = false )
		{
			if ( !entity.IsValid() )
				return false;

			if ( IsCarryingType( entity.GetType() ) )
				return false;

			return base.Add( entity, makeActive );
		}

		public bool IsCarryingType( Type t )
		{
			return List.Any( x => x?.GetType() == t );
		}

		public override bool Drop( Entity ent )
		{
			if ( !Host.IsServer )
				return false;

			if ( !Contains( ent ) )
				return false;

			ent.OnCarryDrop( Owner );

			return ent.Parent == null;
		}
	}
}
