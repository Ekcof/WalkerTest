using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum SimpleFollowMode
{
	Lerp,
	SmoothDamp
}

[DisallowMultipleComponent]
public class SimpleFollower : MonoBehaviour, IMovement
{
	[Header("Follow Settings")]
	[SerializeField] private float _moveSpeed = 3f;               // For Lerp
	[SerializeField] private float _smoothTime = 0.15f;           // For SmoothDamp
	[SerializeField] private float _deadZone = 0.05f;             // Distance to stop moving

	[SerializeField] private SimpleFollowMode _followMode = SimpleFollowMode.Lerp;
	[SerializeField] private LookType _defaultLookType = LookType.None;
	[Header("Target (optional)")]
	[SerializeField] private Transform _target;
	[SerializeField] private bool _lockZ = true;
	[SerializeField] private float _fixedZ = -10f;

	private Transform _followTarget;
	private CancellationTokenSource _followCts;
	private Vector3 _velocity = Vector3.zero;
	private LookType _currentLookType;
	private bool _isFollowing;


	public Transform Target
	{
		get => _target;
		set
		{
			if (_target != value)
			{
				_target = value;
				if (isActiveAndEnabled && _target != null)
					Follow(_target);
				else
					Stop();
			}
		}
	}

	private void Start()
	{
		if (_target != null)
			Follow(_target);
	}

	public void Move(Vector2 position, LookType lookType = LookType.None)
	{
		Stop();
		_currentLookType = (lookType == LookType.None) ? _defaultLookType : lookType;
		MoveToPositionAsync(position).Forget();
	}

	public void Follow(Transform target, LookType lookType = LookType.None)
	{
		Stop();
		if (target == null) return;

		_followTarget = target;
		_currentLookType = (lookType == LookType.None) ? _defaultLookType : lookType;
		_isFollowing = true;
		_followCts = new CancellationTokenSource();

		FollowTargetAsync(_followCts.Token).Forget();
	}

	public void ApplyMovement(Vector2 dir)
	{
		transform.position += (Vector3)dir * _moveSpeed * Time.deltaTime;
	}
	private void ApplyPosition(Vector3 newPos)
	{
		if (_lockZ)
			newPos.z = _fixedZ;
		transform.position = newPos;
	}

	public void Stop()
	{
		_followCts?.Cancel();
		_followCts = null;
		_followTarget = null;
		_isFollowing = false;
	}

	public void LookAt(Vector2 targetPosition)
	{
		UpdateLookDirection(targetPosition, true);
	}

	private async UniTaskVoid MoveToPositionAsync(Vector2 target)
	{
		while (true)
		{
			Vector3 pos = transform.position;
			float distance = Vector2.Distance(pos, target);
			if (distance < _deadZone)
				break;

			Vector3 nextPos;
			switch (_followMode)
			{
				case SimpleFollowMode.Lerp:
					nextPos = Vector3.Lerp(pos, target, _moveSpeed * Time.deltaTime);
					break;
				case SimpleFollowMode.SmoothDamp:
					nextPos = Vector3.SmoothDamp(pos, target, ref _velocity, _smoothTime, Mathf.Infinity, Time.deltaTime);
					break;
				default:
					nextPos = target;
					break;
			}
			ApplyPosition(nextPos);
			UpdateLookDirection(target);
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
		}
	}

	private async UniTaskVoid FollowTargetAsync(CancellationToken token)
	{
		while (_isFollowing && _followTarget != null && !token.IsCancellationRequested)
		{
			Vector3 pos = transform.position;
			Vector3 target = _followTarget.position;
			float distance = Vector3.Distance(pos, target);

			if (distance > _deadZone)
			{
				Vector3 nextPos;
				switch (_followMode)
				{
					case SimpleFollowMode.Lerp:
						nextPos = Vector3.Lerp(pos, target, _moveSpeed * Time.deltaTime);
						break;
					case SimpleFollowMode.SmoothDamp:
						nextPos = Vector3.SmoothDamp(pos, target, ref _velocity, _smoothTime, Mathf.Infinity, Time.deltaTime);
						break;
					default:
						nextPos = target;
						break;
				}
				ApplyPosition(nextPos);
				UpdateLookDirection(target);
			}
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);
		}
	}


	private void UpdateLookDirection(Vector2 targetPosition, bool force = false)
	{
		if (_currentLookType == LookType.None && !force) return;

		var dir = targetPosition - (Vector2)transform.position;
		if (dir.sqrMagnitude < 0.0001f) return;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		if (_currentLookType == LookType.Target || force)
			transform.rotation = Quaternion.Euler(0, 0, angle);
	}

	private void OnDestroy()
	{
		_followCts?.CancelAndDispose();
		_followTarget = null;
		_isFollowing = false;
		_followCts = null;
	}
}