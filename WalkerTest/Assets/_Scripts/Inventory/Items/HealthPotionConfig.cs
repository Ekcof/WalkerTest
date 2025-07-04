using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
	[CreateAssetMenu(fileName = "HealthPotionWrapper", menuName = "ScriptableObjects/Items/HealthPotionWrapper")]
	public class HealthPotionConfig : ItemConfig<HealthPotion>
	{
	}
	[System.Serializable]
	public class HealthPotion : Item
	{
		[SerializeField] private int _healthRestored;

		public override bool TryToUse(ICharacter character)
		{
			Debug.Log($"Try To use it on {character.Name} with health restoration: {_healthRestored}");
			if (character.Health == null)
				return false;

			return character.Health.TryApplyChange(_healthRestored);
		}
	}
}
