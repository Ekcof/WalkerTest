using System;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
	[Flags]
	public enum SpawnPointType
	{
		Player = 1 << 0,
		Red = 1 << 1,
		Green = 1 << 2,
		Blue = 1 << 3,
		Yellow = 1 << 4
	}

	public interface ISpawnPoint
	{
		bool IsFreeForSpawn(SpawnPointType pointType);
		bool IsFreeForSpawn();
	}

	public class SpawnPoint : MonoBehaviour, ISpawnPoint
	{
		[Inject] private ICharacterRegistry _characters;
		[SerializeField] private SpawnPointType _pointType;
		[SerializeField] private float _freeForSpawnRadius = 1f;

		public bool IsFreeForSpawn(SpawnPointType pointType)
		{
			return _pointType.HasAnyCommon(pointType)
				&& IsFreeForSpawn();
		}

		public bool IsFreeForSpawn()
		{
			if (_characters == null)
				return false;

			return !_characters.TryGetTargetsInRadius(transform.position, _freeForSpawnRadius, out var targets);
		}
	}
}