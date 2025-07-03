using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Scene.Fight
{
	public class PlayerGun : MonoBehaviour, IGun
	{
		[Inject] private IBulletRegistry _bulletRegistry;
		[Inject] private IBulletConfigHolder _bulletConfigHolder;

		[Inject] private Player _player;
		[SerializeField] private int _ammo;
		private IBulletConfig _currentConfig;
		public int Ammo => _ammo;
		public IBulletConfig CurrentConfig => _currentConfig;

		public Vector2 MuzzlePosition => transform.position; // TODO: Add point for muzzle

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
			_bulletRegistry.Shoot(gunner, position);
		}
	}
}