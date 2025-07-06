using UI.Controls;
using Scene.Character;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
	[Inject] private IJoyStick _joyStick;
	[Inject] private Player _player;
	[SerializeField] private Movement _movement;

	private void Awake()
	{
		_joyStick.Direction.SkipLatestValueOnSubscribe().Subscribe(OnDirectionChanged).AddTo(this);
	}

	private void OnDirectionChanged(Vector2 direction)
	{
		if (_player.IsDead)
		{
			return;
		}

		if (direction.sqrMagnitude > 0.01f)
		{
			_movement.ApplyMovement(direction);
		}
		else
		{
			_movement.Stop();
		}
	}


}
