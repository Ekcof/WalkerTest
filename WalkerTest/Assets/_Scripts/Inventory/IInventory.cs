using Serialization;
using System;
using System.Collections.Generic;

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
		IObservable<IItem> OnItemAdded { get; }
		bool TryGetItem(string id, out IItem item);
		void AddItems(IEnumerable<IItem> items);
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
		void SetItems(IEnumerable<SerializedItem> items);
		int GetAmountById(string id);
	}
}