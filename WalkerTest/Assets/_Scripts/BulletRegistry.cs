using ComponentUtils;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Scene.Fight
{
    public interface IBulletRegistry
    {
        void Shoot(Transform shooter, Transform target, string key);
		void Shoot(IGunner gunner, Vector2 position);
	}


	public class BulletRegistry : MonoBehaviour, IBulletRegistry
    {
        [Inject] private IBulletConfigHolder _configs;
		[Inject] private DiContainer _diContainer;

		[SerializeField] private List<IBullet> _activeBullets = new();
		[SerializeField] private Bullet _bulletPrefab;
		private Pool<Bullet> _bulletPool;

		private void Awake()
		{
			_bulletPool = new(_bulletPrefab, transform, _diContainer);
		}

		public void Shoot(Transform shooter, Transform target, string key)
		{
            var config = _configs.GetConfigById(key);
            var bullet = _bulletPool.Pop();

			bullet.transform.position = shooter.position;
            bullet.ApplyConfig(config)
            .FireAtPosition(target.position);
			_activeBullets.Add(bullet);
		}

		public void Shoot(IGunner gunner, Vector2 position)
		{
			var bullet = _bulletPool.Pop();
			bullet.transform.position = gunner.Collider.transform.position;
			bullet.ApplyConfig(gunner.Gun.CurrentConfig).
				SetShooter(gunner).
				FireAtPosition(position);
			_activeBullets.Add(bullet);
		}

		private void Update()
		{
			float dt = Time.deltaTime;
			for (int i = _activeBullets.Count - 1; i >= 0; i--)
			{
				var bullet = _activeBullets[i];
				if (bullet.Tick(dt))
					continue;

				_activeBullets.RemoveAt(i);
				bullet.Hide();
				_bulletPool.Push((Bullet)bullet);
			}
		}
	}
}