using ComponentUtils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
	/// <summary>
	/// Responsible for spawning of certain kind of enemy at certain spawn points
	/// </summary>
	public interface IEnemySpawner
	{
		string Id { get; }
		IEnumerable<Character> AliveEnemies { get;}
		UniTask StartSpawn(CancellationToken token);
		bool HasEnemy(Character enemy);
		void SpawnEnemy(SpawnPoint point);
		void DespawnEnemy(Character character);
	}
	public class EnemySpawner : IEnemySpawner
	{
		private EnemySpawnConfig _config;
		private Pool<Character> _pool;
		private SpawnPoint[] _spawnPoints;
		private readonly List<Character> _activeEnemies = new();
		public IEnumerable<Character> AliveEnemies => _activeEnemies.Where(_activeEnemies => !_activeEnemies.IsDead);
		public string Id => _config.ID;

		public EnemySpawner(EnemySpawnConfig config, SpawnPoint[] spawnPoints, DiContainer container, Transform parent)
		{
			_config = config;
			_spawnPoints = spawnPoints;
			_pool = new Pool<Character>(config.Prefab, parent, container);
		}

		public async UniTask StartSpawn(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					Debug.Log("Wait 0");
					await UniTask.Delay(TimeSpan.FromSeconds(_config.RespawnDelay), cancellationToken: token);
					Debug.Log("Wait 1");
					await UniTask.WaitUntil(() => _spawnPoints.Any(s => s.IsFreeForSpawn()), cancellationToken: token);
					Debug.Log("Wait 2");
					await UniTask.WaitUntil(() => AliveEnemies.Count() < _config.MaxCount, cancellationToken: token);

				}
				catch
				{
					return;
				}
				var points = _spawnPoints.Where(s => s.IsFreeForSpawn(_config.SpawnPointType));
				var point = points.GetRandomElement();
				CheckDeadBodies();
				SpawnEnemy(point);
			}
		}

		public void DespawnEnemy(Character character)
		{
			_activeEnemies.Remove(character);
			_pool.Push(character);
		}

		public void SpawnEnemy(SpawnPoint point)
		{
			var enemy = _pool.Pop();
			enemy.transform.position = point.transform.position;
			enemy.Refresh();
			_activeEnemies.Add(enemy);
		}

		public bool HasEnemy(Character enemy)
		{
			return _activeEnemies.Any(e => e.Hash.Equals(enemy.Hash));
		}

		private void CheckDeadBodies()
		{
			var delay = _config.RemoveDeadDelay;
			if (delay <= 0)
				return;

			var deads = _activeEnemies.Where(_activeEnemies => _activeEnemies.IsDead);

			for (int i = deads.Count() - 1; i >= 0; i--)
			{
				var dead = deads.ElementAt(i);
				if (dead.TimeOfDeath + delay < Time.time)
				{
					DespawnEnemy(dead);
					Debug.Log($"Despawned dead enemy {dead.Hash} after {delay} seconds.");
				}
			}
		}
	}
}

