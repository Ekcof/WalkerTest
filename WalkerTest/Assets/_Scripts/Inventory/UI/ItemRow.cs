using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public interface IItemRow
	{
		IEnumerable<IItemSlot> AllSlots { get; }
		int SlotsNumber { get; }
		void UpdateRow(IItem[] items);
		void ClearSlots();
	}

	public class ItemRow : MonoBehaviour, IItemRow
	{
		[SerializeField] private ItemSlot[] _slots;
		public int SlotsNumber => _slots.Length;
		public IEnumerable<IItemSlot> AllSlots => _slots;

		public void ClearSlots()
		{
			foreach (var item in _slots)
			{
				item.RemoveSelection();
			}
		}

		public void UpdateRow(IItem[] items)
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				if (i < items.Length)
				{
					_slots[i].UpdateView(items[i]);
				}
				else
				{
					_slots[i].UpdateView(null);
				}
			}
		}
	}
}