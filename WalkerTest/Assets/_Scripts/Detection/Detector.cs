using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Scene.Character;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Scene.Detection
{

	public interface IDetector
	{
		IReadOnlyReactiveCollection<ITarget> Targets { get; }
		bool HasTargets { get; }
		ITarget NearestTarget { get; }
		void ToggleDetection(bool isActive, IEnumerable<Type> targetTypes);
		bool IsInMeleeDistance();
	}

	public class Detector : MonoBehaviour, IDetector
	{
		[Inject] private ICharacterRegistry _characterRegistry;

		[SerializeField] private float _detectionRadius = 5f;
		[SerializeField] private float _meleeRadius = 0.5f;
		[SerializeField] private float _detectionInterval = 0.5f;

		private ReactiveCollection<ITarget> _targets = new();
		private CancellationTokenSource _detectionCts;

		private bool IsDetecting => _detectionCts != null && !_detectionCts.IsCancellationRequested;
		public IReadOnlyReactiveCollection<ITarget> Targets => _targets;

		public bool HasTargets => Targets.Count > 0;

		public ITarget NearestTarget
		{
			get
			{
				ITarget nearest = null;
				float minDist = float.MaxValue;
				foreach (var target in _targets)
				{
					float dist = target.GetDistance(transform.position);
					if (dist < minDist)
					{
						minDist = dist;
						nearest = target;
					}
				}
				return nearest;
			}
		}

		public void ToggleDetection(bool isActive, IEnumerable<Type> targetTypes)
		{
			if (!isActive)
			{
				_detectionCts?.CancelAndDispose();
				_detectionCts = null;
			}
			else if (!IsDetecting)
			{
				_detectionCts = new();
				StartDetection(targetTypes, _detectionCts.Token).Forget();
			}
		}

		private async UniTask StartDetection(IEnumerable<Type> targetTypes, CancellationToken token)
		{
			while (_detectionCts != null && !token.IsCancellationRequested)
			{
				if (_characterRegistry.TryGetTargetsInRadius(targetTypes, transform.position, _detectionRadius, out var characters))
				{
					RefreshTargets(characters);
				}
				else if(HasTargets)
				{
					_targets.Clear();
				}

				try
				{
					await UniTask.Delay(TimeSpan.FromSeconds(_detectionInterval), cancellationToken: token);
				}
				catch
				{
					return;
				}
			}
		}

		private void RefreshTargets(IEnumerable<ICharacter> characters)
		{
			var currentCharacterIds = new HashSet<string>(characters.Select(c => c.Hash)); // или c.UniqueId, если есть

			for (int i = _targets.Count - 1; i >= 0; i--)
			{
				Debug.Log($"Check target with hash {_targets[i].Character.Hash} against current characters: {string.Join(", ", currentCharacterIds)}");
				if (!currentCharacterIds.Contains(_targets[i].Character.Hash))
				{
					Debug.Log($"[SYNC] Remove target: {_targets[i].Character.Name} ({_targets[i].Character.Hash})");
					_targets.RemoveAt(i);
				}
			}

			foreach (var v in characters)
			{
				if (!_targets.Any(b => b.Character.Hash.Equals(v.Hash)))
				{
					Debug.Log($"[SYNC] Add target: {v.Name} ({v.Hash})");
					_targets.Add(new Target(v));
				}
			}
		}

		private void OnDestroy()
		{
			_detectionCts?.CancelAndDispose();
		}

		public bool IsInMeleeDistance()
		{
			return NearestTarget?.GetDistance(transform.position) <= _meleeRadius;
		}
	}
}
