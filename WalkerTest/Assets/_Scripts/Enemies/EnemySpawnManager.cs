using ComponentUtils;
using Cysharp.Threading.Tasks;
using Scene.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
	[Serializable]
	public class EnemySpawnConfig
	{
		public string ID;
		public Character Prefab;
		public bool SpawnMaxOnAwake;
		public int MaxCount = 3;
		public int TotalCount = 30;
		public float RespawnDelay = 1f;
		public float RemoveDeadDelay = 3f;
		public SpawnPointType SpawnPointType;
	}

	public interface IEnemySpawnManager : IInitializable, IDisposable
	{
		void TryToDespawnCharacter(Character character);
	}

	public class EnemySpawnManager : MonoBehaviour, IEnemySpawnManager
	{
		[Inject] private DiContainer _diContainer;

		[SerializeField] private EnemySpawnConfig[] _spawnConfigs;
		private readonly List<EnemySpawner> _spawners = new();

		public IEnumerable<Character> AllAliveEnemies => _spawners.SelectMany(b => b.AliveEnemies);
		private List<SpawnPoint> _spawnPoints = new();
		private CancellationTokenSource _cts;

		public void Initialize()
		{
			Debug.Log($"Initialize for {_spawnConfigs.Length} configs");
			_spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None).ToList();
			_cts?.CancelAndDispose();
			_cts = new();

			foreach (var config in _spawnConfigs)
			{
				var points = _spawnPoints.Where(b => b.IsFreeForSpawn(config.SpawnPointType)).ToArray();
				var spawner = new EnemySpawner(config, points, _diContainer, transform);
				_diContainer.Inject(spawner);
				spawner.StartSpawn(_cts.Token).Forget();
				_spawners.Add(spawner);
			}
			Debug.Log($"Created {_spawners.Count} _spawners");
		}

		private Character SpawnEnemy(Pool<Character> pool, SpawnPoint point)
		{
			var enemy = pool.Pop();
			enemy.Refresh();
			enemy.transform.position = point.transform.position;

			return enemy;
		}

		public void TryToDespawnCharacter(Character character)
		{
			if (character == null)
				return;

			foreach (var spawner in _spawners)
			{
				if (spawner.HasEnemy(character))
				{
					spawner.DespawnEnemy(character);
					break;
				}

			}
		}

		public void Dispose()
		{
			_cts?.CancelAndDispose();
		}

	}
}