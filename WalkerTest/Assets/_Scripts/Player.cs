using Inventory;
using Scene.Detection;
using Scene.Fight;
using Scene.UI;
using Serialization;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
    public class Player : Character, IGunner
	{
		private const float MIN_AIM_INTERVAL = 0.5f;

		[Inject] private IItemHolderRegistry _itemHolderRegistry;
		[Inject] private IPlayerLog _playerLog;
		[SerializeField] private PlayerGun _gun;
		[SerializeField] private Detector _detector;
		[SerializeField] private ValueBar _healthBar;

		[SerializeField] private Transform _weaponLeftPos;
		[SerializeField] private Transform _weaponRightPos;

		private int _experience;

		// States
		private IdleState _idleState = new();
		private ShootingState _shootingState = new();
		private PlayerMoveState _moveState = new();
		private DeadState _deadState = new();

		private float _lastShotTime;

		private bool HasShot => Time.time - _lastShotTime < MIN_AIM_INTERVAL;
		public IGun Gun => _gun;
		public override float CurrentDamage => 50f; // Melee damage

		public Vector2 LeftPosition => _weaponLeftPos.position;

		public Vector2 RightPosition => _weaponRightPos.position;
		public int Experience => _experience;

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

			_lastShotTime = Time.time;

			if (_detector.NearestTarget == null)
			{
				_playerLog.AddMessage("No targets in range");
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

			if (Health.Current.Value <= 0)
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
				if (HasShot)
				{
					SetState(_shootingState);
				}
				else
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

		public void Respawn()
		{
			Health.Refresh();
			SetState(_idleState);
		}


		public void Deserialize(PlayerSerializationData data)
		{
			if (data == null)
			{
				Debug.LogError("Data is null");
				return;
			}

			_experience = data.Experience;
			Inventory.SetItems(data.Items);
			if (data.Health > 0)
			{
				Health.SetValue(data.Health);
			}
		}

		public PlayerSerializationData Serialize()
		{
			var items = new List<SerializedItem>();
			foreach (var item in Inventory.AllItems)
			{
				items.Add(new SerializedItem
				{
					Id = item.Id,
					Amount = item.Amount
				});
			}

			var save = new PlayerSerializationData
			{
				Experience = _experience,
				Health = Health.Current.Value,
				Items = items
			};
			return save;
		}
	}
}