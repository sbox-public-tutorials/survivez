using Sandbox;
using System.Linq;

namespace survivez.Entities
{
	// BaseNPC holds the movement logic, nothing about the AI and such.
	public abstract partial class BaseNPC : AnimEntity
	{
		public string Model { get; set; } = "models/citizen/citizen.vmdl";

		public float MaxHealth { get; set; } = 10.0f;
		public bool CanDie { get; set; } = true;

		public bool IsAlive { get => CanDie && Health > 0; }

		public float Speed { get; private set; }

		[ServerCmd( "npc_clear" )]
		public static void NPCClear()
		{
			foreach ( var npc in All.OfType<NPC>().ToArray() )
				npc.Delete();
		}

		public BaseNPC()
		{
		}

		public override void Spawn()
		{
			base.Spawn();

			Health = MaxHealth;
			SetModel( Model );
			EyePos = Position + Vector3.Up * 64;
			CollisionGroup = CollisionGroup.Player;
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );

			EnableHitboxes = true;

			Speed = Rand.Float( 100, 300 );
		}
	}
} // namespace survivez.Entities

