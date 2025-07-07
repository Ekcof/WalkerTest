using Inventory;
using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;

namespace Scene.Fight
{
	public class PlayerGun : MonoBehaviour, IGun
	{
		private const string DEFAULT_AMMO_TYPE = "DefaultAmmo";

		[Inject] private IBulletRegistry _bulletRegistry;
		[Inject] private IBulletConfigHolder _bulletConfigHolder;
		[Inject] private IPlayerLog _log;

		[Inject] private Player _player;
		[SerializeField] private int _ammo;
		private IBulletConfig _currentConfig;

		private IInventory Inventory => _player.Inventory;
		public string AmmoId => DEFAULT_AMMO_TYPE; // TODO: make it configurable
		public int Ammo => _ammo;
		public IBulletConfig CurrentConfig => _currentConfig;

		private void Awake()
		{
			_currentConfig = _bulletConfigHolder.GetConfigById("Bullet");
		}

		public void SetAmmo(int ammo)
		{
			throw new System.NotImplementedException();
		}

		public void Shoot(IGunner gunner, Vector2 position)
		{
			if (Inventory.TryRemoveItem(DEFAULT_AMMO_TYPE, 1))
			{
				_bulletRegistry.Shoot(gunner, position);
			}
			else
			{
				_log.AddMessage("Out of ammo!");
			}
		}
	}
}