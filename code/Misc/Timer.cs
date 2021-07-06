using Sandbox;
using System;
using System.Collections.Generic;

namespace survivez.Misc
{
	public class TimerManager
	{
		internal static TimerManager Singleton;

		internal List<InternalTimer> Timers { get; set; } = new List<InternalTimer>();

		public TimerManager()
		{
			Singleton = this;
		}

		public void Update()
		{
			if ( Singleton == null )
				return;

			for ( var i = Timers.Count; i > 0; --i )
			{
				InternalTimer timer = Timers[i-1];
				if ( timer.IsCompleted() )
				{
					timer.Invoke();
					if (timer.ShouldDelete())
					{
						Timers.RemoveAt( i-1 );
					}
				}
			}
		}

		internal static void AddInternalTimer( InternalTimer _internalTimer )
		{
			if ( Singleton == null )
				return;

			Singleton.Timers.Add( _internalTimer );
		}
	}

	public static class Timer
	{
		public static readonly float Second		= 1000.0f;
		public static readonly float Minute		= 60 * Second;
		public static readonly float Hour		= 60 * Minute * Second;
		public static readonly float Day		= 24 * 60 * Minute * Second;

		public static void Simple(float _delayMs, Action _action) 
		{
			Create( _delayMs, 1, _action );
		}
		public static void Frame( Action _action )
		{
			Create( 0.0f, 1, _action );
		}
		public static void Create( float _delayMs, int _repeatTimes, Action _action ) 
		{
			InternalTimer timer = new( _delayMs, _repeatTimes, _action );
			TimerManager.AddInternalTimer( timer );
		}
	}

	internal class InternalTimer
	{
		public bool Infinite { get; private set; }
		public int RepeatTimes { get; private set; } = 0;
		public TimeSince CompleteTime { get; private set; }
		private float DelayMs { get; set; }
		public Action Action;

		public InternalTimer( float _delayMs, int _repeatTimes, Action _action)
		{
			CompleteTime = 0.0f;
			DelayMs = _delayMs/1000.0f;
			RepeatTimes = _repeatTimes;
			Action = _action;

			if ( _repeatTimes == 0 )
			{
				Infinite = true;
			}
		}

		public void Invoke()
		{
			if ( Action != null )
			{
				if ( Infinite || RepeatTimes > 0 )
				{
					Action.Invoke();
				}
				RepeatTimes--;
			}
		}

		public bool ShouldDelete()
		{
			return !Infinite && RepeatTimes <= 0;
		}
		public bool IsCompleted()
		{
			return CompleteTime > DelayMs;
		}
	}
}
