using Scene.Character;
using UnityEngine;

namespace Scene.Detection
{
    public interface ITarget
    {
        ICharacter Character { get; }
        Transform Transform { get; }
		Vector3 Position { get; }
		int Priority { get; }
		float GetDistance(Vector3 position);
		float GetDistance(Vector2 position);
	}

    public class Target : ITarget
    {
		public ICharacter Character { get; private set; }
		public int Priority { get; private set; }
		public Transform Transform => Character.Transform;
		public Vector3 Position => Character.Position;

		public Target(ICharacter character, int priority = 0)
		{
			Character = character;
			Priority = priority;
		}

		public float GetDistance(Vector3 position)
		{
			return Vector3.Distance(position, Position);
		}

		public float GetDistance(Vector2 position)
		{
			return Vector2.Distance(position, Position);
		}
	}
}