using Sandbox;

namespace survivez.Nav
{
	public class Follow : NavSteer
	{
		public Entity FollowTarget { get; set; }
		public float MinRadius { get; set; } = 200;
		public float MaxRadius { get; set; } = 500;

		public float Tolerance { get; set; } = 35.0f;

		public Follow( Entity _owner, Entity _target ) : base(_owner)
		{
			FollowTarget = _target;
		}

		public override void Tick( Vector3 position )
		{
			base.Tick( position );

			if ( FollowTarget != null && FollowTarget.IsValid() )
			{
				Target = FollowTarget.Position + ((Owner.Position - FollowTarget.Position).WithZ( 0 ).Normal * Tolerance);
				//DebugOverlay.Line( FollowTarget.Position, FollowTarget.Position + ((Owner.Position - FollowTarget.Position).WithZ( 0 ).Normal * Tolerance) );
				//DebugOverlay.Axis( Target, Rotation.Identity );
			}
			else
			{
				if (Path.IsEmpty)
				{
					// Wander...
					RandomNearTarget( position );
				}
			}
		}

		public virtual bool RandomNearTarget( Vector3 center )
		{
			var t = NavMesh.GetPointWithinRadius( center, MinRadius, MaxRadius );
			if ( t.HasValue )
			{
				Target = t.Value;
			}

			return t.HasValue;
		}


	}
}
