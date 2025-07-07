using Inventory;
using Scene.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using System;

namespace UI.Controls
{
	public class FireButton : MonoBehaviour
	{
		[Inject] private Player _player;
		[SerializeField] private Button _button;
		[SerializeField] private TMP_Text _ammoText;
		private IInventory Inventory => _player.Inventory;
		private void Awake()
		{
			_button.SetListener(OnFireButtonClicked);
			Inventory.OnItemAdded
				.Subscribe(UpdateAmmoText)
				.AddTo(this);
		}

		private void UpdateAmmoText(IItem item = null)
		{
			_ammoText.text = $"x{Inventory.GetAmountById(_player.Gun.AmmoId)}";
		}

		private void OnFireButtonClicked()
		{
			_player.TryToShoot();
			UpdateAmmoText();
		}
	}
}