using Inventory;
using Scene.Detection;
using Scene.Fight;
using Scene.UI;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
    public class Player : Character, IGunner
	{
		[Inject] private IItemHolderRegistry _itemHolderRegistry;
		[SerializeField] private PlayerGun _gun;
		[SerializeField] private Detector _detector;
		[SerializeField] private ValueBar _healthBar;
		public IGun Gun => _gun;
		public override float CurrentDamage => 50f; // Melee damage

		private void Awake()
		{
			_healthBar.ApplyValue(Health);
			_detector.ToggleDetection(true, ~TargetType.Player);
		}

		public bool TryToShoot()
		{
			if (_gun == null)
			{
				Debug.LogError("Gun is not assigned.");
				return false;
			}
			if (_gun.Ammo <= 0)
			{
				Debug.LogWarning("No ammo left.");
				return false;
			}

			_gun.Shoot(this, _detector.NearestTarget.Position);
			return true;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (_detector.HasItemHolders)
			{
				// TODO: FIX MULTIPLY TAKING
				var nearest = _detector.NearestItemHolder;
				if (nearest != null)
				{
					Debug.Log($"____TAKE ITEMS {nearest.Hash} {nearest.Items.Count}");
					var items = nearest.Items;
					Inventory.AddItems(items);
					_itemHolderRegistry.Unregister(nearest);
				}
			}
		}
	}
}