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
			Duration = 0.5f;
			AllowCarry = false;
		}

		// This is what happens when the "animation" starts.
		public override void OnStart()
		{
			var pawn = GetPawn();
			if ( pawn?.Inventory != null )
			{
				CurrentWeapon = pawn.Inventory.Active;
				pawn.ActiveChild = null;
			}

			SetAnimInt( "holdtype_hand", 2);
			SetAnimFloat( "holdtype_attack", 4);
			SetAnimInt( "holdtype", 4 );
			SetAnimInt( "holdtype_handedness", 1 );
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
