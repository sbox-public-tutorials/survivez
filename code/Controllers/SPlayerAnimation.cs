using Sandbox;
using survivez.Controllers.Animations;
using survivez.Misc;
using System;
using System.Collections.Generic;

namespace survivez.Controllers
{
	public enum AnimationStates : byte
	{
		Base = 0,
		Throwing = 1,
		Ducking = 2,
	}
	public partial class SPlayerAnimation
	{

		public static SPlayerAnimation CreateAnimation(AnimationStates id)
		{
			switch ( id )
			{
				case AnimationStates.Base:
					break;
				case AnimationStates.Throwing:
					return new SPlayerAnimationThrow();
				case AnimationStates.Ducking:
					return new SPlayerAnimationDuck();
				default:
					break;
			}
			return null;
		}

		public SPlayerAnimator Animator { get; set; }

		public bool IsPlaying { get; set; } = false;

		public float StartTime {get;set;}

		public float StopTime { get; set; }

		public bool AllowCarry { get; set; } = true;

		Dictionary<string, int> AnimInts = new Dictionary<string, int>();
		Dictionary<string, float> AnimFloats = new Dictionary<string, float>();
		Dictionary<string, bool> AnimBools = new Dictionary<string, bool>();

		public SPlayerAnimation()
		{

		}


		public float Duration { get; set; }
		public AnimationStates AnimationState { get; set; }
		public bool Interruptable { get; set; } = true;

		public void Start(SPlayerAnimator animator)
		{
			if (IsPlaying)
			{
				Stop();
			}

			IsPlaying = true;
			Animator = animator;
			var pawn = animator.Pawn;
			var animPawn = animator.AnimPawn;
			StartTime = Time.Now;
			StopTime = Time.Now + Duration;

			animator.AnimationState = AnimationState;
			if ( Duration > 0 )
			{
				OnStart();
				Timer.Frame( () =>
				{
					Timer.Simple( Duration * Timer.Second, () =>
					{
						Stop();
					} );
				} );
			} else
			{
				OnStart();
			}
		}

		public void SetAnimInt(string name, int val )
		{
			var animPawn = GetAnimPawn();
			if ( !AnimInts.ContainsKey( name ) )
			{
				AnimInts[name] = animPawn.GetAnimInt( name );
			}
			animPawn.SetAnimInt( name, val );
		}
		public void SetAnimBool( string name, bool val )
		{
			var animPawn = GetAnimPawn();
			if ( !AnimBools.ContainsKey( name ) )
			{
				AnimBools[name] = animPawn.GetAnimBool( name );
			}
			animPawn.SetAnimBool( name, val );
		}
		public void SetAnimFloat( string name, float val )
		{
			var animPawn = GetAnimPawn();
			if ( !AnimFloats.ContainsKey( name ) )
			{
				AnimFloats[name] = animPawn.GetAnimFloat( name );
			}
			animPawn.SetAnimFloat(name, val);
		}

		public void DisposeAnimState()
		{
			var animPawn = GetAnimPawn();
			if ( animPawn != null )
			{
				foreach ( KeyValuePair<string, bool> item in AnimBools )
				{
					animPawn.SetAnimBool( item.Key, item.Value );
				}
				foreach ( KeyValuePair<string, int> item in AnimInts )
				{
					animPawn.SetAnimInt( item.Key, item.Value );
				}
				foreach ( KeyValuePair<string, float> item in AnimFloats )
				{
					animPawn.SetAnimFloat( item.Key, item.Value );
				}

				ClearAnimStates();
			}
			Animator.AnimationState = AnimationStates.Base;
		}

		public void ClearAnimStates()
		{
			AnimFloats = new Dictionary<string, float>();
			AnimBools = new Dictionary<string, bool>();
			AnimInts = new Dictionary<string, int>();
		}


		public Entity GetPawn()
		{
			if (Animator == null)
			{
				return null;
			}
			return Animator.Pawn;
		}
		public AnimEntity GetAnimPawn()
		{
			if ( Animator == null )
			{
				return null;
			}
			return Animator.AnimPawn;
		}

		public void Simulate()
		{
			if ( IsPlaying )
			{
				var now = Time.Now;
				var duration = now - StartTime;
				var StopTimeLocal = StopTime - now;
				var percentage = 1f;
				if ( StopTimeLocal > 0 )
				{
					percentage = duration / StopTimeLocal;
				}

				OnSimulate( duration, percentage );
			}
		}


		public virtual void OnSimulate(float duration, float percentage) { }
		public virtual void OnStop()	{	}
		public virtual void OnStart() { }

		public void Stop()
		{
			
			if ( IsPlaying )
			{				
				OnStop();
				DisposeAnimState();
			}
			IsPlaying = false;
		}
	}
}
