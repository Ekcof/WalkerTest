using Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Inventory
{
	public class MainInventory : MonoBehaviour, IInventory
	{
		[Inject] private DiContainer _diContainer;
		[Inject] private IItemConfigHolder _itemConfigHolder;

		[SerializeField] private List<ItemToAdd> _itemsToAdd;
		private bool _hasDeserializedItems;

		protected List<IItem> InternalItems = new();
		public IEnumerable<IItem> AllItems => InternalItems;

		public IEnumerable<IInventory> NestedInventories => throw new System.NotImplementedException();


		protected virtual void Awake()
		{
			if (!_hasDeserializedItems)
			{
				AddItems(_itemsToAdd);
			}
		}

		protected void AddItems(IEnumerable<ItemToAdd> itemsToAdd)
		{
			Debug.Log($"Add items {itemsToAdd.Count()}");
			foreach (var itemToAdd in itemsToAdd)
			{
				var item = _itemConfigHolder.GetItemConfigById(itemToAdd.Id)?.GetCopy(_diContainer, itemToAdd.Amount);
				if (item != null)
				{
					TryAddItem(item);
				}
				else
				{
					Debug.LogWarning($"Item config for ID {itemToAdd.Id} not found.");
				}
			}
		}

		public void AddItems(IEnumerable<IItem> items)
		{
			Debug.Log($"Add items {items.Count()}");
			foreach (var item in items)
			{
				TryAddItem(item);
			}
		}

		public bool TryAddItem(IItem item)
		{
			if (item == null)
			{
				Debug.LogError("Cannot add a null item to the inventory.");
				return false;
			}

			// add non stackable item
			if (!item.IsStackable)
			{
				InternalItems.Add(item);
				return true;
			}

			int remainingAmount = item.Amount;
			List<IItem> sameItems = InternalItems.FindAll(i => i.Id == item.Id && i.Amount < i.StackLimit);

			if (item.IsStackable && item.Amount == 0)
			{
				return false;
			}

			// fill existing stacks
			foreach (var stack in sameItems)
			{
				if (remainingAmount <= 0) break;

				int canAdd = Mathf.Min(stack.StackLimit - stack.Amount, remainingAmount);
				stack.AddAmount(canAdd);
				remainingAmount -= canAdd;
			}

			// add new stacks
			while (remainingAmount > 0)
			{
				int amountToAdd = Mathf.Min(item.StackLimit, remainingAmount);
				IItem newItem = item.Copy(_diContainer, amountToAdd);
				InternalItems.Add(newItem);
				remainingAmount -= amountToAdd;
			}

			return true;
		}

		public bool TryGetItem(string id, out IItem item)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveItem(IItem item)
		{
			InternalItems.Remove(item);
		}

		public bool TryRemoveItem(string id, int amount)
		{
			if (amount <= 0)
				return false;

			// Find all stacks of this type by id
			var stacks = InternalItems
				.Where(i => i.Id == id)
				.OrderBy(i => i.Amount) // small stacks first
				.ToList();

			int total = stacks.Sum(i => i.Amount);

			// if it's not enough items to remove
			if (total < amount)
				return false;

			int amountToRemove = amount;

			foreach (var stack in stacks)
			{
				if (amountToRemove <= 0)
					break;

				if (stack.Amount <= amountToRemove)
				{
					amountToRemove -= stack.Amount;
					InternalItems.Remove(stack); // remove the whole stack
				}
				else
				{
					stack.AddAmount(-amountToRemove);
					amountToRemove = 0;
				}
			}

			return true;
		}

		public void SetItems(IEnumerable<SerializedItem> items)
		{
			InternalItems.Clear();
			_hasDeserializedItems = true;
			foreach (var serializedItem in items)
			{
				var itemConfig = _itemConfigHolder.GetItemConfigById(serializedItem.Id);
				if (itemConfig != null)
				{
					var item = itemConfig.GetCopy(_diContainer, serializedItem.Amount);
					if (item != null)
					{
						InternalItems.Add(item);
					}
				}
				else
				{
					Debug.LogWarning($"Item config for ID {serializedItem.Id} not found.");
				}
			}
		}
	}
}