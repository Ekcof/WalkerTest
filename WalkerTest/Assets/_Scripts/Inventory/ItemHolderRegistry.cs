using ComponentUtils;
using Scene.Character;
using Scene.Fight;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;
using Zenject;

namespace Inventory
{
    public interface IItemHolderRegistry
    {
		void Register(ItemHolder holder);
		void Unregister(ItemHolder holder);
		void Unregister(IItemHolder holder);
		bool HasItemHolder(IItemHolder holder);
		void Create(IEnumerable<IItem> items, Vector2 position);
		bool TryGetHoldersInRadius(Vector3 position, float radius, out IEnumerable<IItemHolder> targets);
	}


	public class ItemHolderRegistry : MonoBehaviour, IItemHolderRegistry
	{
		[Inject] private DiContainer _diContainer;

		[SerializeField] private ItemHolder _prefab;
		private Pool<ItemHolder> _pool;

		private List<IItemHolder> _itemHolders = new();
		private void Awake()
		{
			_pool = new(_prefab, transform, _diContainer);
		}

		public void Create(IEnumerable<IItem> items, Vector2 position)
		{
			var holder = _pool.Pop();
			holder.transform.position = position;
			holder.Refresh();
			holder.AddItems(items);
			Register(holder);
		}

		public void Register(ItemHolder holder)
		{
			if (_itemHolders.Contains(holder))
			{
				return;
			}

			holder.SetActive(true);
			_itemHolders.Add(holder);
		}

		public bool HasItemHolder(IItemHolder itemHolder)
		{
			return _itemHolders.Contains(itemHolder);
		}

		public bool TryGetHoldersInRadius(Vector3 position, float radius, out IEnumerable<IItemHolder> targets)
		{
			targets = _itemHolders.Where(t =>
				Vector3.Distance(t.Position, position) <= radius);

			return targets.Any();
		}

		public void Unregister(ItemHolder holder)
		{
			holder.SetActive(false);
			_itemHolders.Remove(holder);
			_pool.Push(holder);
		}

		public void Unregister(IItemHolder holder)
		{
			Unregister((ItemHolder) holder);
		}
	}
}