using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace survivez.Misc
{
	class SRandom
	{

		public static Random rnd = new Random();
		public static float Float()
		{
			return (float)SRandom.rnd.NextDouble();
		}

		public static float Float(float min, float max)
		{
			return (Float() * (max-min)) + min;
		}

		public static int Int( int min, int max )
		{
			return SRandom.rnd.Next( min, max );
		}

	}
}
