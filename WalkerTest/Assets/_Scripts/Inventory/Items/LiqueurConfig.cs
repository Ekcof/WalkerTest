using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
	[CreateAssetMenu(fileName = "Liqueur", menuName = "ScriptableObjects/Items/LiqueurConfig")]
	public class LiqueurConfig : ItemConfig<Liqueur>
	{
	}

	[System.Serializable]
	public class Liqueur : Item
	{
		[SerializeField] private int _healthRestored;

		public override bool TryToUse(ICharacter character)
		{
			if (character.Health == null)
				return false;

			return character.Health.TryApplyChange(_healthRestored);
		}
	}
}