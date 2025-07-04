using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Inventory
{
    public interface IInventory
    {
        IEnumerable<IItem> AllItems { get; }
        IEnumerable<IInventory> NestedInventories { get; }
        bool TryGetItem(string id, out IItem item);
		bool TryAddItem(IItem item);
		void RemoveItem(IItem item);
	}

    public class MainInventory : MonoBehaviour, IInventory
	{
		[Serializable]
		private class ItemToAdd
		{
			public string Id;
			public int Amount;
		}

		[Inject] private DiContainer _diContainer;
		[Inject] private IItemConfigHolder _itemConfigHolder;

		[SerializeField] private List<ItemToAdd> _itemsToAdd;


		private List<IItem> _items = new();
		public IEnumerable<IItem> AllItems => _items;

		public IEnumerable<IInventory> NestedInventories => throw new System.NotImplementedException();


		private void Awake()
		{
			foreach(var itemToAdd in _itemsToAdd)
			{
				var item = _itemConfigHolder.GetItemConfigById(itemToAdd.Id)?.GetCopy(_diContainer, itemToAdd.Amount);

				if (item != null)
				{
					TryAddItem(item);
				}
			}
		}

		public bool TryAddItem(IItem item)
		{
			if (item == null)
			{
				Debug.LogError("Cannot add a null item to the inventory.");
				return false;
			}

			if (item.IsStackable)
			{
				var existingItem = _items.Find(i => i.Id == item.Id);
				if (existingItem != null)
				{
					int totalAmount = existingItem.Amount + item.Amount;
					if (totalAmount > item.StackLimit)
					{
						existingItem.AddAmount(item.StackLimit - existingItem.Amount);
						int remainingAmount = totalAmount - item.StackLimit;
						IItem newItem = item.Copy(_diContainer, remainingAmount);
						_items.Add(newItem);
					}
					else
					{
						existingItem.AddAmount(item.Amount);
					}
					return true;
				}
			}

			_items.Add(item);
			return true;
		}

		public bool TryGetItem(string id, out IItem item)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveItem(IItem item)
		{
			_items.Remove(item);
		}
	}
}