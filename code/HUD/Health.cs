using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace survivez.HUD
{
	public class Health : Panel
	{
		public Label Label;
		private float lastHealth;

		public Health()
		{
			StyleSheet.Load( "/Content/ui/health/health.scss" );

			Label = Add.Label( "???", "value" );

			var player = Local.Pawn;
			if ( player == null ) return;
			lastHealth = player.Health;
		}

		public override void Tick()
		{
			var player = Local.Pawn;
			if ( player == null ) return;
			lastHealth = MathX.LerpTo( lastHealth, player.Health, Time.Delta );
			Label.Text = $"{lastHealth.CeilToInt()}";
		}
	}
}
