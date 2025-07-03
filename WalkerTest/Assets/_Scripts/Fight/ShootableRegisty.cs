using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scene.Fight
{
    public interface IShootableRegistry
    {
        void Register(IShootable shootable);
		void Unregister(IShootable shootable);
		bool TryGetShootableByCollider(Collider2D collider, out IShootable shootable);
	}

    public class ShootableRegisty : IShootableRegistry
    {
		private List<IShootable> _shootables = new();
		public void Register(IShootable shootable)
		{
			if (_shootables.Any(s => shootable.Hash.Equals(s.Hash)))
			{
				return;
			}
			else
			{
				_shootables.Add(shootable);
			}
		}

		public void Unregister(IShootable shootable)
		{
				_shootables.Remove(shootable);
		}

		public bool TryGetShootableByCollider(Collider2D collider, out IShootable shootable)
		{
			Debug.Log($"_____! There are {_shootables.Count} shootables in registry");
			shootable = _shootables.FirstOrDefault(s => s.Collider == collider);
			return shootable != null;
		}

	}
}