using Sandbox;


namespace survivez.Controllers.Animations
{
	partial class SPlayerAnimationThrow: SPlayerAnimation
	{
		Entity CurrentWeapon { get; set; }

		public SPlayerAnimationThrow()
		{
			AnimationState = AnimationStates.Throwing;
			Interruptable = false;
			Duration = 0.25f;
			AllowCarry = false;
		}

		// This is what happens when the "animation" starts.
		public override void OnStart()
		{
			Log.Info( "START" );
			var pawn = GetPawn();
			if ( pawn?.Inventory != null )
			{
				CurrentWeapon = pawn.Inventory.Active;
				pawn.ActiveChild = null;
			}

			SetAnimInt( "holdtype_attack", 1);
			SetAnimInt( "holdtype", 4 );
			SetAnimFloat( "aimat_weight", 1.0f );
			SetAnimFloat( "duck", 1f );
			SetAnimInt( "holdtype_handedness", 5 );
			SetAnimBool( "b_attack", true );
		}

		public override void OnStop()
		{
			if ( CurrentWeapon != null )
			{
				var pawn = GetPawn();
				pawn.ActiveChild = CurrentWeapon;
			}
		}
	}
}
