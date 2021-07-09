using Sandbox;
using System.Linq;

namespace survivez.Entities
{
	public partial class Baracade : ModelEntity
	{

		public Baracade()
		{
			if ( IsServer ) Transmit = TransmitType.Never;
		}

		public override void Spawn()
		{
			SetModel( "models/sbox_props/concrete_barrier/concrete_barrier.vmdl" );
			RenderAlpha = 0.7f;
			RenderDirty();
			base.Spawn();
		}


		public void ShowValid()
		{

			RenderColor = Color.White;
			GlowColor = Color.Green;
		}

		public void ShowInvalid()
		{

			RenderColor = Color.Red;
			GlowColor = Color.Red;
		}


		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}

}
