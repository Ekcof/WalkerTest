using Scene.Detection;
using UnityEngine;
using UniRx;
using System;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;

namespace Scene.Character
{
	public class MonsterMelee : Character
	{
		[SerializeField] private float _currentDamage = 25f;
		private Type[] _targetTypes = new Type[] { typeof(Player) };
		private Detector _detector;

		private IdleState _idleState = new();
		private FollowState _followState = new();
		private MeleeAttackState _attackState = new();
		private ITarget _currentTarget;

		public override float CurrentDamage => _currentDamage;

		private void Awake()
		{
			_detector = GetComponent<Detector>();
			if (_detector == null)
			{
				Debug.LogError("MonsterMelee requires a Detector component.");
				return;
			}

			SetState(_idleState);
			_detector.ToggleDetection(true, _targetTypes);
			_detector.Targets.ObserveAdd().Subscribe(OnTargetAdded).AddTo(this);
			_detector.Targets.ObserveRemove().Subscribe(OnTargetRemoved).AddTo(this);

		}

		private void OnTargetRemoved(CollectionRemoveEvent<ITarget> @event)
		{
			if (!_detector.HasTargets && CurrentState != _idleState)
			{
				SetState(_idleState);
			}
		}

		private void OnTargetAdded(CollectionAddEvent<ITarget> @event)
		{
			var nearestTarget = _detector.NearestTarget;

			if (nearestTarget == null)
			{
				SetState(_idleState);
				return;
			}

			if (nearestTarget != _currentTarget)
			{
				_currentTarget = nearestTarget;
				_followState.SetTarget(_currentTarget);

				SetState(_followState);
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (CurrentState is (FollowState or IdleState) && _currentTarget != null && _detector.IsInMeleeDistance())
			{
				_attackState.SetTarget(_currentTarget);
				SetState(_attackState);
			}
			else if (_detector.NearestTarget == null)
			{
				SetState(_idleState);
			}
			else if (CurrentState is MeleeAttackState)
			{
				if(!_detector.IsInMeleeDistance())
				{
					_followState.SetTarget(_currentTarget);
					SetState(_followState);
				}
			}
			
			CurrentState?.Update();
		}
	}
}