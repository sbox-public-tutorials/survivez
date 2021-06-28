using Sandbox;
using Sandbox.UI;
using survivez.Controllers;
using survivez.Entities;

namespace survivez.HUD.Crosshair
{
	public partial class SCrosshair : Panel
	{
		public const float startingScale = 5;
		public float scale = startingScale;

		public Vector2 position = Vector2.One / 2.0f;

		int fireCounter;

		public SCrosshair()
		{
			StyleSheet.Load( "/Content/ui/crosshair/standardcrosshair.scss" );
			SetClass( "crosshair", true );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not SPlayer pawn )
				return;

			TraceResult tr = Trace.Ray( pawn.EyePos, pawn.EyePos + (pawn.EyeRot.Forward.WithZ( 0 ) * 200.0f) ).Ignore(pawn).Run();
			/*
			if ( tr.Entity is NPC npc )
				Style.BorderColor = Color.Red;
			else
				Style.BorderColor = Color.White;
			*/

			// Trace {-1 - 1}		-- Inheritly has more values available.
			// 1 + {-1 - 1} = ||{0 - 2}|| = {0 - 1} & flip x and y = HTML Panel
			// HTML Panel {0 - 1}	
			Vector2 trToScreen = tr.EndPos.ToScreen();
			//Log.Info( $"TrToScreen	| Output Position" );
			//Log.Info( $"{trToScreen}	| {position}" );
			position = trToScreen;
			float padding = 0.1f;
			position.x = MathX.Clamp( position.x, 0.0f + padding, 1.0f - padding );
			position.y = MathX.Clamp( position.y, 0.0f + padding, 1.0f - padding );

			Style.Top	= new Length()	{ Unit = LengthUnit.Percentage,		Value = position.y * 100.0f };
			Style.Left	= new Length()	{ Unit = LengthUnit.Percentage,		Value = position.x * 100.0f };

			Style.Width = 12 * scale;
			Style.Height = 12 * scale;
			Style.Dirty();

			scale = scale.LerpTo( startingScale, Time.Delta * 5 );
			SetClass( "fire", fireCounter > 0 );

			if ( fireCounter > 0 )
				fireCounter--;
		}

		[PanelEvent("fire")]
		public void FireEvent()
		{
			scale = 10;
			fireCounter += 2;
		}
	}
}
