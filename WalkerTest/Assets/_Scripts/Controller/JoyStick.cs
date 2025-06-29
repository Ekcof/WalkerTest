using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UniRx;

namespace Controller
{
	public interface IJoyStick
	{
		IReadOnlyReactiveProperty<Vector2> Direction { get; }
		bool IsActive { get; }
		event Action<Vector2> OnDirectionChanged;
	}

	public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IJoyStick
	{
		[SerializeField] private RectTransform _main;
		[SerializeField] private RectTransform _handle;

		private ReactiveProperty<Vector2> _inputDirection = new(Vector2.zero);
		public IReadOnlyReactiveProperty<Vector2> Direction => _inputDirection;
		public bool IsActive => _activePointerId != -1;

		public event Action<Vector2> OnDirectionChanged;

		private int _activePointerId = -1;

		private void Start()
		{
			ResetHandle();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_activePointerId != -1) return;

			if (RectTransformUtility.RectangleContainsScreenPoint(_main, eventData.position, eventData.pressEventCamera))
			{
				_activePointerId = eventData.pointerId;
				UpdateJoystick(eventData);
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.pointerId != _activePointerId) return;
			UpdateJoystick(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.pointerId != _activePointerId) return;
			_activePointerId = -1;
			_inputDirection.Value = Vector2.zero;
			OnDirectionChanged?.Invoke(_inputDirection.Value);
			ResetHandle();
		}

		private void UpdateJoystick(PointerEventData eventData)
		{
			Vector2 localPoint;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_main, eventData.position, eventData.pressEventCamera, out localPoint))
			{
				var radius = _main.sizeDelta.x * 0.5f;
				var clamped = Vector2.ClampMagnitude(localPoint, radius);
				_handle.anchoredPosition = clamped;

				Vector2 newDirection = clamped / radius;
				if (newDirection != _inputDirection.Value)
				{
					_inputDirection.Value = newDirection;
					OnDirectionChanged?.Invoke(_inputDirection.Value);
				}

				Debug.Log($"Joystick Direction: {_inputDirection}");
			}
		}

		private void ResetHandle()
		{
			_handle.anchoredPosition = Vector2.zero;
		}
	}
}