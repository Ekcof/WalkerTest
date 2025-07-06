using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory
{
	public interface IItemConfigHolder
	{
		IItemConfig GetItemConfigById(string id);
	}

	[CreateAssetMenu(fileName = "ItemConfigHolder", menuName = "ScriptableObjects/ItemConfigHolder")]

	public class ItemConfigHolder : ScriptableObject, IItemConfigHolder
	{
		[SerializeField] private List<ItemConfig> _itemConfigs;

		public IItemConfig GetItemConfigById(string id)
		{
			return _itemConfigs.FirstOrDefault(b => b.ID.Equals(id));
		}
	}
}