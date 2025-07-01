using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scene.Character
{
	public interface ICharacterRegistry
	{
		bool TryGetTargetByName(string targetId, out ICharacter target);
		bool TryGetTargets(string targetId, out IEnumerable<ICharacter> targets);
		bool TryGetTargetsInRadius(Type targetType, Vector3 position, float radius, out IEnumerable<ICharacter> targets);
		void RegisterTarget(ICharacter target);
		void UnregisterTarget(ICharacter target);
	}

	public class CharacterRegistry : MonoBehaviour, ICharacterRegistry
	{
		private readonly List<ICharacter> _targetList = new List<ICharacter>();

		private void Awake()
		{  
			var charactersOnScene = FindObjectsOfType<Character>();
			foreach (var c in charactersOnScene)
			{
				RegisterTarget(c);
			}
		}

		public bool TryGetTargetsInRadius(Type targetType, Vector3 position, float radius, out IEnumerable<ICharacter> targets)
		{
			targets = _targetList.Where(t => t.GetType() == targetType && Vector3.Distance(t.Position, position) <= radius);
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

		public void RegisterTarget(ICharacter target)
		{
			if (!_targetList.Contains(target))
				_targetList.Add(target);
		}

		public void UnregisterTarget(ICharacter target)
		{
			_targetList.Remove(target);
		}
	}
}