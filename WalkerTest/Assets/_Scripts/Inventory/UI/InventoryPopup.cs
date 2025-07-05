using ComponentUtils;
using Scene.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using Zenject;


namespace Inventory
{
	public class InventoryPopup : MonoBehaviour, IPopup
	{
		[Inject] private DiContainer _diContainer;
		[Inject] private Player _player;

		[SerializeField] private ItemRow _itemRowPrefab;
		[SerializeField] private RectTransform _content;
		private Pool<ItemRow> _rowsPool;
		private List<ItemRow> _rows = new();
		private CompositeDisposable _compositeDisposable = new();

		private IItemSlot _selectedSlot;
		private IEnumerable<IItemSlot> Slots => _rows.SelectMany(row => row.AllSlots);

		public bool IsOpen => gameObject.activeSelf;

		private void Awake()
		{
			_rowsPool = new Pool<ItemRow>(_itemRowPrefab, _content, _diContainer);
		}

		public IPopup Open()
		{
			gameObject.SetActive(true);
			UpdateRows();
			return this;
		}

		public IPopup Close()
		{
			_compositeDisposable.Clear();
			gameObject.SetActive(false);
			return this;
		}

		private void ClearRows()
		{
			_compositeDisposable.Clear();
			_rows.ForEach(row =>
			{
				_rowsPool.Push(row);
				row.gameObject.SetActive(false);
				row.ClearSlots();
			});
			_rows.Clear();
		}

		private void UpdateRows()
		{
			ClearRows();
			var items = _player.Inventory.AllItems.ToArray();

			int slotsPerRow = _itemRowPrefab.SlotsNumber;
			int totalRows = Mathf.CeilToInt((float)items.Length / slotsPerRow);

			for (int i = 0; i < totalRows; i++)
			{
				var row = _rowsPool.Pop();
				int startIndex = i * slotsPerRow;
				int endIndex = Mathf.Min(startIndex + slotsPerRow, items.Length);
				var itemsForRow = items.Skip(startIndex).Take(endIndex - startIndex).ToArray();
				row.UpdateRow(itemsForRow);
				_rows.Add(row);
				foreach (var slot in row.AllSlots)
				{
					slot.IsSelected
						.SkipLatestValueOnSubscribe()
						.Subscribe(OnSelectSlot)
						.AddTo(_compositeDisposable);
					slot.Item
						.Pairwise()
						.Subscribe(OnChangeItem)
						.AddTo(_compositeDisposable);
				}
			}
		}

		private void OnChangeItem(Pair<IItem> pair)
		{
			Debug.Log($"____On Change intem {pair.Current?.Id} {pair.Previous?.Id}");
			var prev = pair.Previous;
			if (prev != null && pair.Current == null)
			{
				UpdateRows();
			}
		}

		private void OnSelectSlot(IItemSlot selected)
		{
			if (_selectedSlot == null)
			{
				_selectedSlot = selected;
			}
			else if (selected == _selectedSlot)
			{
				_selectedSlot = null;
			}
			else
			{
				_selectedSlot.RemoveSelection();
				_selectedSlot = selected;
			}

			foreach (var slot in Slots)
			{
				if (slot != _selectedSlot)
				{
					slot.RemoveSelection();
				}
			}
		}

		private void OnDestroy()
		{
			_compositeDisposable?.Dispose();
		}
	}
}
