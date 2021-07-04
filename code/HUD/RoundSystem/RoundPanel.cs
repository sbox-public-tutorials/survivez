using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using survivez.Controllers;

namespace survivez.HUD
{
	public class RoundPanel : Panel
	{
		public PhasePanel PhaseP { get; set; }

		public float RoundPercentage { get; set; }

		public RoundPanel()
		{
			StyleSheet.Load( "/Content/ui/round-system/RoundSystem.scss" );
			PhaseP = AddChild<PhasePanel>();
		}

		public override void Tick()
		{
			PhaseP.Tick();
		}
	}

	public class PhasePanel : Panel
	{
		public Label PhaseLabel;

		public Label RoundLabel;
		public LoadingBar PhaseBar;

		public PhasePanel()
		{
			PhaseLabel = Add.Label( "Warming Up", "value" );
			PhaseBar = AddChild<LoadingBar>();
			RoundLabel = Add.Label();
			RoundLabel.SetClass( "round-label", true );
		}

		public override void Tick()
		{
			if ( Local.Pawn is SPlayer player )
			{
				var now = Time.Now;
				var timeLeft = player.PhaseEndTime - now;
				var totalDuration = player.PhaseEndTime - player.PhaseStartTime;
				var difference = totalDuration - timeLeft;
				var phaseName = "Warm Up";
				var percentage = 0f;
				if ( totalDuration > 0 )
				{
					percentage = timeLeft / totalDuration;
				}

				if ( player.CurrentPhase == 1 )
				{
					phaseName = "Preparation";
				}
				else if ( player.CurrentPhase == 2 )
				{
					phaseName = "Defending";
				}
				PhaseLabel.SetText( $"{phaseName}" );
				PhaseBar.SetValue( percentage * 100 );
				RoundLabel.SetText( $"Round: {player.CurrentRound}" );
			}
			else
			{
				PhaseLabel.SetText( "Error" );
			}

		}


	}
	public class LoadingBar : Panel
	{
		public Panel Container;

		public Panel Inner;

		public float Current = 0;

		public LoadingBar()
		{
			Container = AddChild<Panel>();
			Inner = Container.AddChild<Panel>();
			Container.SetClass( "loading-bar-container", true );
			Inner.SetClass( "loading-bar-inner", true );

		}

		public void SetValue( float percentage )
		{
			Current = percentage.Clamp( 0, 100 );
			Inner.Style.Width = Length.Percent( (float)Current );
			Inner.Style.Dirty();
		}
	}
}