using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scene.Fight
{
    public interface IShootableRegistry
    {
        void RegisterShootable(IShootable shootable);
		void UnregisterShootable(string hash);
		bool TryGetShootableByCollider(Collider2D collider, out IShootable shootable);
	}

    public class ShootableRegisty : IShootableRegistry
    {
		private List<IShootable> _shootables = new();
		public void RegisterShootable(IShootable shootable)
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

		public void UnregisterShootable(string hash)
		{
			var element = _shootables.FirstOrDefault(s => s.Hash.Equals(hash));
		if (element != null)
			{
				_shootables.Remove(element);
			}
		}

		public bool TryGetShootableByCollider(Collider2D collider, out IShootable shootable)
		{
			shootable = _shootables.FirstOrDefault(s => s.Collider == collider);
			return shootable != null;
		}

	}
}