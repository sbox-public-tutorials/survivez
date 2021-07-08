using Sandbox;


namespace survivez.Controllers.Animations
{
	partial class SPlayerAnimationDuck: SPlayerAnimation
	{

		float DuckDuration = 0.2f;
		public SPlayerAnimationDuck()
		{
			AnimationState = AnimationStates.Ducking;
		}

		// This is what happens when the "animation" starts.
		public override void OnStart()
		{
			// Sets the default and also makes it save the end state.
			SetAnimFloat( "duck", 0f );
		}

		public override void OnSimulate( float duration, float percentage ) {
			var lerp = duration / DuckDuration;
			if (lerp > 1)
			{
				lerp = 1;
			}
			SetAnimFloat( "duck", lerp );
		}

	}
}
