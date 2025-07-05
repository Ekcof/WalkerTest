using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

namespace UI
{
	public interface IPlayerLogSlot
	{
		void Show(string message, Action onComplete);
		void Hide(Action onComplete);
	}

	public class PlayerLogSlot : MonoBehaviour, IPlayerLogSlot
	{
		[SerializeField] private TMP_Text _text;
		[SerializeField] private float _lifeTime = 5f;
		[SerializeField] private CanvasGroup _canvasGroup;

		[Header("Animation")]
		[SerializeField] private float _fadeDuration = 0.5f;
		[SerializeField] private float _moveDistance = 40f;
		[SerializeField] private float _moveDuration = 0.5f;

		private Tween _tween;

		public void Show(string message, Action onComplete)
		{
			// Move to bottom in hierarchy (UI order)
			_text.text = message;

			// Kill any previous animations
			_tween?.Kill();
			gameObject.SetActive(true);
			transform.SetAsLastSibling();

			// Prepare for animation
			_canvasGroup.alpha = 0f;
			RectTransform rect = transform as RectTransform;
			Vector2 startPos = rect.anchoredPosition;
			Vector2 downPos = startPos - Vector2.up * _moveDistance; // move down

			rect.anchoredPosition = downPos;
			_tween = DOTween.Sequence()
				.Append(rect.DOAnchorPos(startPos, _moveDuration).SetEase(Ease.OutCubic))
				.Join(_canvasGroup.DOFade(1f, _fadeDuration))
				.AppendInterval(_lifeTime)
				.OnComplete(() =>
				{
					onComplete?.Invoke();
				});
		}

		public void Hide(Action onComplete)
		{
			_tween?.Kill();
			_tween = _canvasGroup.DOFade(0f, 0.5f)
				.OnComplete(() => { gameObject.SetActive(false); onComplete?.Invoke(); });
		}
	}
}