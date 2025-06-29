using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum LookType
{
	None,
	Target,
	Path
}

public interface IMovement
{
	void Move(Vector2 position, LookType lookType = LookType.None);
	void Follow(Transform target, LookType lookType = LookType.None);
	void ApplyMovement(Vector2 dir);
	void Stop();
	void LookAt(Vector2 targetPosition);
}


public class Movement : MonoBehaviour, IMovement
{
	[SerializeField] private float _moveSpeed = 3f;
	[SerializeField] private Ease _moveEase = Ease.Linear;
	[SerializeField] private LookType _defaultLookType = LookType.None;
	[SerializeField] private Rigidbody2D _rb;

	private Vector2 _moveDirection = Vector2.zero;
	private Tween _moveTween;
	private Transform _followTarget;
	private LookType _currentLookType;
	private bool _isFollowing;
	private CancellationTokenSource _followCts;

	private void Awake()
	{
		_rb ??= GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (_moveDirection != Vector2.zero)
		{
			Vector2 newPosition = _rb.position + _moveDirection * _moveSpeed * Time.fixedDeltaTime;
			_rb.MovePosition(newPosition);
		}
	}

	public void ApplyMovement(Vector2 dir)
	{
		_moveDirection = dir.normalized;
	}

	public void Move(Vector2 position, LookType lookType = LookType.None)
	{
		Stop();
		_currentLookType = (lookType == LookType.None) ? _defaultLookType : lookType;

		Vector2 start = _rb.position;
		float distance = Vector2.Distance(start, position);
		float duration = distance / _moveSpeed;

		_moveTween = DOTween.To(() => _rb.position, x => {
			_rb.MovePosition(x);
			UpdateLookDirection(position); // Поворот
		}, position, duration)
		.SetEase(_moveEase)
		.OnComplete(() => _moveTween = null);
	}

	public void Follow(Transform target, LookType lookType = LookType.None)
	{
		Stop();
		if (target == null) return;

		_followTarget = target;
		_currentLookType = (lookType == LookType.None) ? _defaultLookType : lookType;
		_isFollowing = true;
		_followCts = new CancellationTokenSource();

		FollowAsync(_followCts.Token).Forget();
	}

	public void Stop()
	{
		_moveTween?.Kill();
		_moveTween = null;
		_followTarget = null;
		_isFollowing = false;
		_followCts?.Cancel();
		_followCts = null;
		_moveDirection = Vector2.zero;
	}

	public void LookAt(Vector2 targetPosition)
	{
		UpdateLookDirection(targetPosition, true);
	}

	private async UniTaskVoid FollowAsync(CancellationToken token)
	{
		const float FOLLOW_REPATH_THRESHOLD = 0.05f;

		while (_isFollowing && _followTarget != null && !token.IsCancellationRequested)
		{
			Vector2 pos = _rb.position;
			Vector2 targetPos = (Vector2)_followTarget.position;
			float distance = Vector2.Distance(pos, targetPos);

			if (distance > FOLLOW_REPATH_THRESHOLD)
			{
				float duration = distance / _moveSpeed;
				_moveTween?.Kill();
				var tcs = new UniTaskCompletionSource();
				_moveTween = DOTween.To(() => _rb.position, x =>
				{
					_rb.MovePosition(x);
					UpdateLookDirection(targetPos);
				}, targetPos, duration)
					.SetEase(_moveEase)
					.OnComplete(() =>
					{
						_moveTween = null;
						tcs.TrySetResult();
					});

				await tcs.Task.AttachExternalCancellation(token);
			}
			else
			{
				try
				{
					await UniTask.Delay(50, cancellationToken: token);
				}
				catch
				{
					break;
				}
			}
		}
	}

	private void UpdateLookDirection(Vector2 targetPosition, bool force = false)
	{
		if (_currentLookType == LookType.None && !force) return;

		var dir = targetPosition - _rb.position;
		if (dir.sqrMagnitude < 0.0001f) return;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		if (_currentLookType == LookType.Target || force)
			_rb.transform.rotation = Quaternion.Euler(0, 0, angle);
		// Path — можно добавить отдельно
	}
}