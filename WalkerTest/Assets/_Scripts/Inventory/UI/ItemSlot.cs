using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static UnityEditor.Progress;

namespace Inventory
{
	public interface IItemSlot
	{
		IReadOnlyReactiveProperty<IItem> Item { get; }
		IReadOnlyReactiveProperty<IItemSlot> IsSelected { get; }
		void UpdateView(IItem item);
		void RemoveSelection();
		void Hide();
	}

	public class ItemSlot : MonoBehaviour, IItemSlot
	{
		[Inject] private Player _player;
		[SerializeField] private Image _icon;
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private Button _selectButton;
		[SerializeField] private Button _useButton;
		[SerializeField] private Button _dropButton;

		private ReactiveProperty<IItem> _item = new();
		private ReactiveProperty<IItemSlot> _isSelected = new();
		public IReadOnlyReactiveProperty<IItem> Item => _item;
		public IReadOnlyReactiveProperty<IItemSlot> IsSelected => _isSelected;

		private void Awake()
		{
			_selectButton.SetListener(OnSelect);
			_useButton.SetListener(OnUse);
			_dropButton.SetListener(OnDrop);
		}

		private void OnSelect()
		{
			_isSelected.Value = this;
			_useButton.gameObject.SetActive(true);
			_dropButton.gameObject.SetActive(true);
		}

		private void OnUse()
		{
			if (_item.Value != null && _item.Value.TryToUse(_player))
			{
				RemoveItem();
			}
		}

		private void OnDrop()
		{
			if (_item.Value != null)
			{
				Debug.Log($"Dropping item: {_item.Value.Id}");
				RemoveItem();
			}
		}

		private void RemoveItem()
		{
			_player.Inventory.RemoveItem(_item.Value);
			_item.Value = null;
		}

		public void UpdateView(IItem item)
		{
			if (item == null)
			{
				Hide();
			}
			else
			{
				_item.Value = item;
				RemoveSelection();
				gameObject.SetActive(true);
				var text = item.IsStackable && item.Amount > 1 ?
				$"x{item.Amount}" :
					string.Empty;
				_amountText.text = text;
				_icon.sprite = item.Sprite;
			}
		}

		public void Hide()
		{
			_item.Value = null;
			_isSelected.Value = null;
			_icon.sprite = null;
			_amountText.text = string.Empty;
			gameObject.SetActive(false);
		}

		public void RemoveSelection()
		{
			_useButton.gameObject.SetActive(false);
			_dropButton.gameObject.SetActive(false);
			_isSelected.Value = null;
		}
	}
}