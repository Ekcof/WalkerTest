using Scene.Fight;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scene.Character
{
	public interface ICharacterRegistry
	{
		bool TryGetTargetByName(string targetId, out ICharacter target);
		bool TryGetTargets(string targetId, out IEnumerable<ICharacter> targets);
		bool TryGetTargetsInRadius(TargetType targetType, Vector3 position, float radius, out IEnumerable<ICharacter> targets);
		void Register(ICharacter target);
		void Unregister(ICharacter target);
	}

	public class CharacterRegistry : MonoBehaviour, ICharacterRegistry
	{
		private readonly List<ICharacter> _targetList = new();

		private void Awake()
		{  
			//var charactersOnScene = FindObjectsOfType<Character>();
			//foreach (var c in charactersOnScene)
			//{
			//	RegisterTarget(c);
			//}
		}

		public bool TryGetTargetsInRadius(TargetType targetType, Vector3 position, float radius, out IEnumerable<ICharacter> targets)
		{
			targets = _targetList.Where(t =>
				t.TargetType.HasAnyCommon(targetType) &&  
				Vector3.Distance(t.Position, position) <= radius);

			return targets.Any();
		}

		public bool TryGetTargetByName(string name, out ICharacter target)
		{
			target = _targetList.FirstOrDefault(b => b.Name.Equals(name));
			return target != null && target != default;
		}

		public bool TryGetTargets(string targetId, out IEnumerable<ICharacter> targets)
		{
			targets = _targetList.Where(t => t.ID.Equals(targetId));
			return targets.Count() > 0;
		}

		public void Register(ICharacter target)
		{
			if (!_targetList.Contains(target))
				_targetList.Add(target);
		}

		public void Unregister(ICharacter target)
		{
			_targetList.Remove(target);
		}
	}
}