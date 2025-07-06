using Inventory;
using Scene.Detection;
using Scene.Fight;
using Scene.UI;
using UI;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
    public class Player : Character, IGunner
	{
		[Inject] private IItemHolderRegistry _itemHolderRegistry;
		[Inject] private IPlayerLog _playerLog;
		[SerializeField] private PlayerGun _gun;
		[SerializeField] private Detector _detector;
		[SerializeField] private ValueBar _healthBar;

		[SerializeField] private Transform _weaponLeftPos;
		[SerializeField] private Transform _weaponRightPos;

		private IdleState _idleState = new();
		private ShootingState _shootingState = new();
		private PlayerMoveState _moveState = new();
		private DeadState _deadState = new();

		public IGun Gun => _gun;
		public override float CurrentDamage => 50f; // Melee damage

		public Vector2 LeftPosition => _weaponLeftPos.position;

		public Vector2 RightPosition => _weaponRightPos.position;

		private void Awake()
		{
			_healthBar.ApplyValue(Health);
			_detector.ToggleDetection(true, ~TargetType.Player);
		}

		public bool TryToShoot()
		{
			if (IsDead)
				return false;

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

		private void CheckItemHolders()
		{
			if (_detector.HasItemHolders)
			{
				var nearest = _detector.NearestItemHolder;
				if (nearest != null && _itemHolderRegistry.HasItemHolder(nearest))
				{
					var items = nearest.Items;
					Inventory.AddItems(items);
					foreach (var item in items)
					{
						_playerLog.AddMessage($"Picked up {item.Id} ({item.Amount})", Color.green);
					}
					_itemHolderRegistry.Unregister(nearest);
				}
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (CurrentState == _deadState)
			{
				return;
			}

			if (Health.CurrentValue.Value <= 0)
			{
				if (CurrentState != _deadState)
				{
					SetState(_deadState);
				}
				return;
			}

			_currentTarget = _detector.NearestTarget;

			if (Movement.CurrentDirection == Vector2.zero)
			{
				if (CurrentState != _idleState)
				{
					SetState(_idleState);
				}
			}
			else if (CurrentState != _moveState)
			{
				if (CurrentState != _moveState)
				{
					_moveState.SetWeapon(_gun);
					SetState(_moveState);
				}
			}

			CheckItemHolders();

			CurrentState?.Update();
		}
	}
}