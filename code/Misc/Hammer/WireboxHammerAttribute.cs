using Hammer;
using System;
using System.Text;

namespace survivez.Misc.Hammer
{
	[AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
	public class WireboxHammerAttribute : MetaDataAttribute
	{
		internal byte MinX, MinY, MinZ;
		internal byte MaxX, MaxY, MaxZ;

		public override void AddHeader( StringBuilder sb )
		{
			// Todo :: ??? Or Request this???
			sb.Append( $" wirebox(\"{MinX} {MinY} {MinZ}\", \"{MaxX} {MaxY} {MaxZ}\")" );
		}
	}
}
