using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public interface IItem
    {
        string Id { get; }
        int Amount { get; }
        bool IsStackable { get; }
		void Use();
	}

    public class Item : IItem
	{
		public string Id => throw new System.NotImplementedException();

		public int Amount => throw new System.NotImplementedException();

		public bool IsStackable => throw new System.NotImplementedException();

		public void Use()
		{
			throw new System.NotImplementedException();
		}
    }
}