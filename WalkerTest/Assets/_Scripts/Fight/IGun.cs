using Scene.Fight;
using UnityEngine;

namespace Scene.Fight
{
	public interface IGun
	{
		IBulletConfig CurrentConfig { get; }
		void SetAmmo(int ammo);
		void Shoot(IGunner gunner, Vector2 position);
	}
}