using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Inventory
{
	[Serializable]
	public class ItemToAdd
	{
		public string Id;
		public int Amount;
	}
	public interface IInventory
	{
		IEnumerable<IItem> AllItems { get; }
		IEnumerable<IInventory> NestedInventories { get; }
		bool TryGetItem(string id, out IItem item);
		void AddItems(IEnumerable<IItem> items);
		bool TryAddItem(IItem item);
		/// <summary>
		/// Try to remove certain amount of item from inventory (good for stackable items).
		/// </summary>
		/// <param name="id"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		bool TryRemoveItem(string id, int amount);
		/// <summary>
		/// Remove certain item from inventory (good for unstackable items).
		/// </summary>
		/// <param name="item"></param>
		void RemoveItem(IItem item);
	}

	public class MainInventory : MonoBehaviour, IInventory
	{
		[Inject] private DiContainer _diContainer;
		[Inject] private IItemConfigHolder _itemConfigHolder;

		[SerializeField] private List<ItemToAdd> _itemsToAdd;


		private List<IItem> _items = new();
		public IEnumerable<IItem> AllItems => _items;

		public IEnumerable<IInventory> NestedInventories => throw new System.NotImplementedException();


		private void Awake()
		{
			foreach (var itemToAdd in _itemsToAdd)
			{
				var item = _itemConfigHolder.GetItemConfigById(itemToAdd.Id)?.GetCopy(_diContainer, itemToAdd.Amount);

				if (item != null)
				{
					TryAddItem(item);
				}
			}
		}

		public void AddItems(IEnumerable<IItem> items)
		{
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
				_items.Add(item);
				return true;
			}

			int remainingAmount = item.Amount;
			List<IItem> sameItems = _items.FindAll(i => i.Id == item.Id && i.Amount < i.StackLimit);

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
				_items.Add(newItem);
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
			_items.Remove(item);
		}

		public bool TryRemoveItem(string id, int amount)
		{
			if (amount <= 0)
				return false;

			// Find all stacks of this type by id
			var stacks = _items
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
					_items.Remove(stack); // remove the whole stack
				}
				else
				{
					stack.AddAmount(-amountToRemove);
					amountToRemove = 0;
				}
			}

			return true;
		}
	}
}