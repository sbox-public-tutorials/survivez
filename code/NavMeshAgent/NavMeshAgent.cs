using Sandbox;
using System.Collections.Generic;
using survivez.Entities;

namespace survivez.NavMeshAgent
{
    public static class NavMeshSettings
    {
            [ConVar.Replicated]
            public static bool nav_drawpath { get; set; }
    }

    public partial struct NavMeshPath
    {
        public struct NavMeshOutput
        {
            public bool Finished;
            public Vector3 Direction;
        }

	    public NavMeshOutput Output;

        public List<Vector3> PathPoints { get; set; }
        private Vector3? targetPosition;
		public Vector3? TargetEndPosition { get => targetPosition; set { Log.Info($"Setting TargetEndPosition {value}"); PathPoints = new List<Vector3>(); targetPosition = value; } }

        private Vector3 NextPosition { get; set; }

        public bool HasPath => PathPoints != null && PathPoints.Count > 0;

        public void Update( Vector3 currentPosition )
        {
            CalculateMoveDirection( currentPosition );
        }

        private void CalculateMovePath( Vector3 currentPosition, Vector3 nextPosition )
        {
			if ( TargetEndPosition == null )
				return;

			using ( Sandbox.Debug.Profile.Scope( $"NavMesh Calculate Move Path {PathPoints.Count}" ) )
			{
				bool shouldBuild = false;

				if ( !TargetEndPosition.Value.IsNearlyEqual( nextPosition, 5 ) )
				{
					TargetEndPosition = nextPosition;
					shouldBuild = true;
				}

				if ( shouldBuild )
				{
					PathPoints.Clear();
					NavMesh.BuildPath( currentPosition, NextPosition, PathPoints );
				}

				if ( PathPoints.Count <= 1 )
					return;

				var deltaToCurrent = currentPosition - PathPoints[0];
				var deltaToNext = currentPosition - PathPoints[1];
				var delta = deltaToNext - deltaToCurrent;
				var deltaNormal = delta.Normal;

				DebugOverlay.ScreenText( 5, $"Delta: {delta} | DeltaNormal: {deltaNormal}" );

				if ( deltaToNext.WithZ( 0 ).Length < 20 )
				{
					PathPoints.RemoveAt( 0 );
					return;
				}

				// If we're in front of this line then
				// remove it and move on to next one
				if ( deltaToNext.Normal.Dot( deltaNormal ) >= 1.0f )
				{
					PathPoints.RemoveAt( 0 );
				}
			}
        }

        private void CalculateMoveDirection( Vector3 currentPosition )
        {
            using ( Sandbox.Debug.Profile.Scope( "Update Path" ) )
            {
                CalculateMovePath( currentPosition, NextPosition );
            }

            Output.Finished = !HasPath;

            if ( Output.Finished )
			{
				Output.Direction = Vector3.Zero;
                return;
            }

			using ( Sandbox.Debug.Profile.Scope( "Update Direction" ) )
            {
                Output.Direction = GetDirection( currentPosition );
            }

            var avoid = GetAvoidance( currentPosition, 500 );
            if ( !avoid.IsNearlyZero() )
            {
                Output.Direction = (Output.Direction + avoid).Normal;
            }
        }

        private Vector3 GetAvoidance( Vector3 position, float radius )
        {
            var center = position + Output.Direction * radius * 0.5f;

            var objectRadius = 200.0f;
            Vector3 avoidance = default;

            foreach ( var ent in Physics.GetEntitiesInSphere( center, radius ) )
            {
                if ( ent is not NPC ) continue;
                if ( ent.IsWorld ) continue;

                var delta = (position - ent.Position).WithZ( 0 );
                var closeness = delta.Length;
                if ( closeness < 0.001f ) continue;
                var thrust = ((objectRadius - closeness) / objectRadius).Clamp( 0, 1 );
                if ( thrust <= 0 ) continue;

                //avoidance += delta.Cross( Output.Direction ).Normal * thrust * 2.5f;
                avoidance += delta.Normal * thrust * thrust;
            }

            return avoidance;
        }

        public void DebugDraw( float time, float opacity = 1.0f )
        {
			if ( PathPoints == null )
				return;

            var draw = Sandbox.Debug.Draw.ForSeconds( time );
            var lift = Vector3.Up * 2;

            draw.WithColor( Color.White.WithAlpha( opacity ) ).Circle( lift + NextPosition, Vector3.Up, 20.0f );

            int i = 0;
            var lastPoint = Vector3.Zero;
            foreach ( var point in PathPoints )
            {
                if ( i > 0 )
                {
                    draw.WithColor( i == 1 ? Color.Green.WithAlpha( opacity ): Color.Cyan.WithAlpha( opacity ) ).Arrow( lastPoint + lift, point + lift, Vector3.Up, 5.0f );
                }

                lastPoint = point;
                i++;
            }
        }

        public void DebugDrawPath()
        {
            using ( Sandbox.Debug.Profile.Scope( "Path Debug Draw" ) )
            {
                DebugDraw( 0.1f, 0.1f );
            }
        }


        public float Distance( int point, Vector3 from )
        {
            if ( PathPoints.Count <= point ) return float.MaxValue;

            return PathPoints[point].WithZ( from.z ).Distance( from );
        }

        public Vector3 GetDirection( Vector3 position )
        {
            if ( PathPoints.Count == 1 )
            {
                return (PathPoints[0] - position).WithZ(0).Normal;
            }

            return (PathPoints[1] - position).WithZ( 0 ).Normal;
        }
    }

} // namespace survivez.NavMesh
