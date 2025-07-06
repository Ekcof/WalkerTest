using Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Inventory
{
	[Serializable]
	public class PossibleLoot
	{
		public List<ItemToAdd> Items;
	}

	public class MonsterInventory : MainInventory, IInventory
	{
		[Inject] private IItemConfigHolder _itemConfigHolder;

		[SerializeField] private List<PossibleLoot> _possibleLoot;

		protected override void Awake()
		{
			if (_possibleLoot.Count == 0)
			{
				return;
			}

			// Explicitly specify the overload by providing the second parameter as false  
			var loot = _possibleLoot.GetRandomElement(true);

			if (loot != null && loot.Items != null && loot.Items.Count > 0)
			{
				AddItems(loot.Items);
			}
			else
			{
				Debug.LogWarning("No valid loot found in MonsterInventory.");
			}
		}
	}
}