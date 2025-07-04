using Scene.Character;
using System;
using UnityEngine;

namespace Inventory
{
	[CreateAssetMenu(fileName = "DefaultAmmoWrapper", menuName = "ScriptableObjects/Items/DefaultAmmoWrapper")]
	public class DefaultAmmoConfig : ItemConfig<DefaultAmmo>
	{

	}

	[Serializable]
	public class DefaultAmmo : Item
	{
		public override bool TryToUse(ICharacter character)
		{
			return false;
		}
	}
}