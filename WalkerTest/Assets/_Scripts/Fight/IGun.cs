using Scene.Fight;
using UnityEngine;

namespace Scene.Fight
{
	public interface IGun
	{
		Vector2 MuzzlePosition { get; }
		IBulletConfig CurrentConfig { get; }
		void SetAmmo(int ammo);
		void Shoot(IGunner gunner, Vector2 position);
	}
}