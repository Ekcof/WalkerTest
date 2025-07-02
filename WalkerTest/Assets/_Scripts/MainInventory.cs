using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public interface IInventory
    {
        IEnumerable<IItem> AllItems { get; }
        IEnumerable<IInventory> NestedInventories { get; }
        bool TryGetItem(string id, out IItem item);
	}

    public class MainInventory : MonoBehaviour, IInventory
	{
		public IEnumerable<IItem> AllItems => throw new System.NotImplementedException();

		public IEnumerable<IInventory> NestedInventories => throw new System.NotImplementedException();

		public bool TryGetItem(string id, out IItem item)
		{
			throw new System.NotImplementedException();
		}

    }
}