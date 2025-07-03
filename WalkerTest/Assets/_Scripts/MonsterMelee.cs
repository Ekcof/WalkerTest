using Scene.Detection;
using UnityEngine;
using UniRx;
using Scene.UI;
using Scene.Fight;

namespace Scene.Character
{
	public class MonsterMelee : Character
	{
		[SerializeField] private float _currentDamage = 25f;

		[SerializeField] private ValueBar _healthBar;

		private TargetType _preyTypes = TargetType.Player;
		private Detector _detector;

		private IdleState _idleState = new();
		private FollowState _followState = new();
		private MeleeAttackState _attackState = new();
		private DeadState _deadState = new();
		public override float CurrentDamage => _currentDamage;

		private void Awake()
		{
			_healthBar.ApplyValue(Health);

			_detector = GetComponent<Detector>();
			if (_detector == null)
			{
				Debug.LogError("MonsterMelee requires a Detector component.");
				return;
			}

			SetState(_idleState);
			_detector.ToggleDetection(true, _preyTypes);
			//_detector.Targets.ObserveAdd().Subscribe(OnTargetAdded).AddTo(this);
			//_detector.Targets.ObserveRemove().Subscribe(OnTargetRemoved).AddTo(this);
		}

		private void OnTargetRemoved(CollectionRemoveEvent<ITarget> @event)
		{
			if (!_detector.HasTargets)
			{
				_currentTarget = null;
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
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			if (CurrentState == _deadState)
			{
				return;
			}	

			var nearest = _detector.NearestTarget;

			if (Health.CurrentValue.Value <= 0)
			{
				if (CurrentState != _deadState)
				{
					SetState(_deadState);
				}
				return;
			}

			// If target has been changed
			if (_currentTarget != nearest)
			{
				if (nearest != null)
				{
					_currentTarget = nearest;
					SetState(_followState);
				}
				else
				{
					SetState(_idleState);
				}
			}
			else
			{
				if (_detector.IsInMeleeDistance() && CurrentState != _attackState)
				{
					SetState(_attackState);
				}
				else if (!_detector.IsInMeleeDistance() && CurrentState == _attackState)
				{
					SetState(_followState);
				}
			}

			CurrentState?.Update();
		}
	}
}