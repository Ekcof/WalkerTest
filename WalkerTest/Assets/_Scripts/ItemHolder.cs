using Scene.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
	public interface IItemHolder
	{
		List<IItem> Items { get; }
		void AddItem(IItem item);
		void AddItems(IEnumerable<IItem> items)
		{
			foreach (var item in items)
			{
				AddItem(item);
			}
		}
		void RemoveItem(IItem item);
		void Clear();
		void Refresh();
	}


	public class ItemHolder : MonoBehaviour, IItemHolder
	{
		[SerializeField] private SimpleAnimator _animator;

		private List<IItem> _items = new();
		public List<IItem> Items => _items;

		public void AddItem(IItem item)
		{
			_items.Add(item);

		}

		public void Clear()
		{
			_items.Clear();
		}

		public void RemoveItem(IItem item)
		{
		
		}
		
		public void Refresh()
		{
			Clear();
		}
	}

}