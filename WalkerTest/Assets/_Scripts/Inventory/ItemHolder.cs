using Scene.Character;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Inventory
{
	public interface IItemHolder
	{
		List<IItem> Items { get; }
		Vector3 Position { get; }
		string Hash { get; }
		void AddItem(IItem item);
		void AddItems(IEnumerable<IItem> items);
		void RemoveItem(IItem item);
		void Refresh();
	}


	public class ItemHolder : MonoBehaviour, IItemHolder
	{
		[Inject] private IItemHolderRegistry _itemHolderRegistry;
		[Inject] private IItemConfigHolder _itemConfigHolder;
		[Inject] private DiContainer _diContainer;

		[SerializeField] private SpriteRenderer _renderer;
		[SerializeField] private Sprite _defaultSprite;
		[SerializeField] private List<ItemToAdd> _itemsToAdd;

		private List<IItem> _items = new();
		public string Hash => transform.GetHashCode().ToString();
		public List<IItem> Items => _items;
		public Vector3 Position => transform.position;

		private void Awake()
		{
			foreach (var itemToAdd in _itemsToAdd)
			{
				var item = _itemConfigHolder.GetItemConfigById(itemToAdd.Id)?.GetCopy(_diContainer, itemToAdd.Amount);

				if (item != null)
				{
					AddItem(item);
				}
			}
			if (_itemHolderRegistry != null)
				_itemHolderRegistry.Register(this);
		}

		public void AddItem(IItem item)
		{
			_items.Add(item);
		}

		public void AddItems(IEnumerable<IItem> items)
		{
			foreach (var item in items)
			{
				AddItem(item);
			}
		}

		public void RemoveItem(IItem item)
		{
			if (_items.Contains(item))
			{
				_items.Remove(item);
			}
		}

		public void Refresh()
		{
			_renderer.sprite = _defaultSprite;
			_items.Clear();
		}
	}

}