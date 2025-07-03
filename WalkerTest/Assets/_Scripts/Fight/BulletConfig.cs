using System;
using UnityEngine;

namespace Scene.Fight
{
	[System.Flags]
	public enum TargetType
	{
		Player = 1 << 0,
		Flyers = 1 << 1,
		Melees = 1 << 2
	}

	public interface IBulletConfig
	{
		string Key { get; }
		float LifeTime { get; }
		float Damage { get; }
		float Distance { get; }
		float Speed { get; }
		TargetType[] IgnoreTypes { get; }
	}

	[Serializable]
	public class BulletConfig : IBulletConfig
	{
		[SerializeField] private string _key;
		[SerializeField] private float _lifeTime;
		[SerializeField] private float _damage;
		[SerializeField] private float _distance;
		[SerializeField] private float _speed;
		[SerializeField] private TargetType[] _ignoreTypes;

		public string Key => _key;

		public float LifeTime => _lifeTime;

		public float Damage => _damage;

		public float Distance => _distance;

		public float Speed => _speed;

		public TargetType[] IgnoreTypes => _ignoreTypes;
	}
}