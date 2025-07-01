using Scene.Detection;
using UnityEngine;
using UniRx;
using System;
using Unity.VisualScripting;

namespace Scene.Character
{
	public class MonsterMelee : Character
	{
		[SerializeField] private float _currentDamage = 25f;
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
			}
			CurrentState = _idleState;
			_detector.Targets.ObserveAdd().Subscribe(OnTargetAdded).AddTo(this);
			_detector.Targets.ObserveRemove().Subscribe(OnTargetRemoved).AddTo(this);

		}

		private void OnTargetRemoved(CollectionRemoveEvent<ITarget> @event)
		{
			if (!_detector.HasTargets && !(CurrentState is IdleState))
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
			
			CurrentState?.Update();
		}
	}
}