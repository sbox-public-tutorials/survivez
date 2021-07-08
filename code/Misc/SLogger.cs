using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace survivez.Misc
{
	public class SLogger
	{
		public static void ClientInfo(object data)
		{
			if (Host.IsClient)
			{
				Log.Info( $"[client] {data?.ToString()}" );
			}
		}
		public static void ServerInfo( object data )
		{
			if ( Host.IsServer )
			{
				Log.Info( $"[server] {data?.ToString()}" );
			}
		}
	}
}
