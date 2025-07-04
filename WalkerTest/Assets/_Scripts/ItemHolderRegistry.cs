using Scene.Character;
using Scene.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public interface IItemHolderRegistry
    {
		void Register(IItemHolder target);
		void Unregister(IItemHolder target);
		bool TryGetTargetsInRadius(Vector3 position, float radius, out IEnumerable<IItemHolder> targets);
	}


	public class ItemHolderRegistry : MonoBehaviour, IItemHolderRegistry
	{
		private List<IItemHolder> _itemHolders = new();

		public void Register(IItemHolder target)
		{
			throw new System.NotImplementedException();
		}

		public bool TryGetTargetsInRadius(Vector3 position, float radius, out IEnumerable<IItemHolder> targets)
		{
			throw new System.NotImplementedException();
		}

		public void Unregister(IItemHolder target)
		{
			throw new System.NotImplementedException();
		}
	}
}