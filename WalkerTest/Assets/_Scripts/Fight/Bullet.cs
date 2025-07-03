using Scene.Character;
using System;
using UnityEngine;
using Zenject;

namespace Scene.Fight
{
    public interface IBullet
    {

		IBullet FireAtPosition(Vector2 pos);
		IBullet FireAtDirection(Vector2 dir);
        IBullet ApplyConfig(IBulletConfig config);
		IBullet SetShooter(IGunner shooter);

		bool Tick(float dt);
		void Hide();
    }

    public class Bullet : MonoBehaviour, IBullet
    {
		[Inject] private IShootableRegistry _shootableRegistry;
		[SerializeField] private Movement _movement;
		[SerializeField] private SimpleAnimator _animator;
        [SerializeField] private Collider2D _collider;

        private IBulletConfig _currentConfig;
        private float _lifeTime;
		private Collider2D _shooterCollider;

		public IBullet SetShooter(IGunner shooter)
		{
			if (shooter == null)
			{
				_shooterCollider = null;
			}
			else
			{
				_shooterCollider = shooter.Collider;
			}
			return this;
		}

        public IBullet ApplyConfig(IBulletConfig config)
        {
			_currentConfig = config;
			_lifeTime = config.LifeTime;

			if (_movement == null)
            {
                _movement = GetComponent<Movement>();
                if (_movement == null)
                {
                    _movement = gameObject.AddComponent<Movement>();
                }
            }
            if (_animator == null)
            {
                _animator = GetComponent<SimpleAnimator>();
                if (_animator == null)
                {
                    _animator = gameObject.AddComponent<SimpleAnimator>();
                }
            }

            return this;
        }


		public IBullet FireAtPosition(Vector2 pos)
        {
           var direction = transform.GetDirection(pos);
            _movement.LookAt(pos);
		   FireAtDirection(direction);
            return this;
        }

        public IBullet FireAtDirection(Vector2 dir)
        {
            gameObject.SetActive(true);
            _movement.ApplyMovement(dir);
            _animator?.SetAnimation(_currentConfig.Key);
			return this;

		}
		public bool Tick(float dt)
		{
			_lifeTime -= dt;
			if (_lifeTime <= 0f)
				return false;

			if (_collider != null)
			{
				ContactFilter2D filter = new ContactFilter2D();
				filter.useTriggers = true;

				Collider2D[] results = new Collider2D[4];
				int count = _collider.OverlapCollider(filter, results);

				for (int i = 0; i < count; i++)
				{
					var other = results[i];

					if (other == _collider ||
						(_shooterCollider != null && other == _shooterCollider))
					{
						continue;
					}

					if (_shootableRegistry.TryGetShootableByCollider(other, out var shootable))
					{
						shootable.Health.TryApplyChange(-_currentConfig.Damage);
						return false;
					}
				}
			}
			return true;
		}

		public void Hide()
        {
            gameObject.SetActive(false);
            _movement.Stop();
        }
    }
}