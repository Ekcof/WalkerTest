using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Scene.Character;
using System.Linq;
using System.Collections.Generic;
using System;
using Scene.Fight;
using Inventory;

namespace Scene.Detection
{
	public interface IDetector
	{
		IReadOnlyReactiveCollection<ITarget> Targets { get; }
		IReadOnlyReactiveCollection<IItemHolder> ItemHolders { get; }
		bool HasTargets { get; }
		bool HasItemHolders { get; }
		ITarget NearestTarget { get; }
		IItemHolder NearestItemHolder { get; }
		void ToggleDetection(bool isActive, TargetType targetType);
		bool IsInMeleeDistance();
	}

	public class Detector : MonoBehaviour, IDetector
	{
		[Inject] private ICharacterRegistry _characterRegistry;
		[Inject] private IItemHolderRegistry _itemHolderRegistry;

		[SerializeField] private float _detectionRadius = 5f;
		[SerializeField] private float _closeRadius = 0.5f; // radius for melee attack and items pickup
		[SerializeField] private float _detectionInterval = 0.5f;

		[SerializeField] private bool _isDetectingItemHolders;

		private ReactiveCollection<ITarget> _targets = new();
		private ReactiveCollection<IItemHolder> _itemHolders = new();

		private CancellationTokenSource _detectionCts;

		private bool IsDetecting => _detectionCts != null && !_detectionCts.IsCancellationRequested;
		public IReadOnlyReactiveCollection<ITarget> Targets => _targets;
		public IReadOnlyReactiveCollection<IItemHolder> ItemHolders => _itemHolders;

		public bool HasTargets => Targets.Count > 0;
		public bool HasItemHolders => ItemHolders.Count > 0;

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

		public IItemHolder NearestItemHolder
		{
			get
			{
				IItemHolder nearest = null;
				float minDist = float.MaxValue;
				foreach (var item in _itemHolders)
				{
					float dist = Vector2.Distance(transform.position, item.Position);
					if (dist < minDist)
					{
						minDist = dist;
						nearest = item;
					}
				}
				return nearest;
			}
		}

		public void ToggleDetection(bool isActive, TargetType targetType)
		{
			if (!isActive)
			{
				_detectionCts?.CancelAndDispose();
				_detectionCts = null;
			}
			else if (!IsDetecting)
			{
				_detectionCts = new();
				StartDetection(targetType, _detectionCts.Token).Forget();
			}
		}

		private async UniTask StartDetection(TargetType targetType, CancellationToken token)
		{
			while (_detectionCts != null && !token.IsCancellationRequested)
			{

				if (_characterRegistry.TryGetTargetsInRadius(targetType, transform.position, _detectionRadius, out var characters))
				{
					RefreshTargets(characters);
				}
				else if (HasTargets)
				{
					_targets.Clear();
				}

				if (_isDetectingItemHolders
					&& _itemHolderRegistry.TryGetHoldersInRadius(transform.position, _closeRadius, out var items))
				{
					RefreshItemHolders(items);
				}
				else if (HasTargets)
				{
					_itemHolders.Clear();
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

		private void RefreshItemHolders(IEnumerable<IItemHolder> itemHolders)
		{
			var currentCharacterIds = new HashSet<string>(itemHolders.Select(c => c.Hash)); // или c.UniqueId, если есть

			for (int i = _itemHolders.Count - 1; i >= 0; i--)
			{
				if (!currentCharacterIds.Contains(_itemHolders[i].Hash))
				{
					_itemHolders.RemoveAt(i);
				}
			}

			foreach (var v in itemHolders)
			{
				if (!_itemHolders.Any(b => b.Hash.Equals(v.Hash)))
				{
					_itemHolders.Add(v);
				}
			}
		}

		private void RefreshTargets(IEnumerable<ICharacter> characters)
		{
			var currentCharacterIds = new HashSet<string>(characters.Select(c => c.Hash)); // или c.UniqueId, если есть

			for (int i = _targets.Count - 1; i >= 0; i--)
			{
				if (!currentCharacterIds.Contains(_targets[i].Character.Hash))
				{
					_targets.RemoveAt(i);
				}
			}

			foreach (var v in characters)
			{
				if (!_targets.Any(b => b.Character.Hash.Equals(v.Hash)))
				{
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
			return NearestTarget?.GetDistance(transform.position) <= _closeRadius;
		}
	}
}
