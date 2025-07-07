using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

namespace UI
{
	public interface IPlayerLogSlot
	{
		void Show(string message, Action onComplete, Color color = default);
		void Hide(Action onComplete);
	}

	public class PlayerLogSlot : MonoBehaviour, IPlayerLogSlot
	{
		[SerializeField] private TMP_Text _text;
		[SerializeField] private float _lifeTime = 5f;
		[SerializeField] private CanvasGroup _canvasGroup;

		[Header("Animation")]
		[SerializeField] private float _fadeDuration = 0.5f;

		private Tween _tween;

		public void Show(string message, Action onComplete, Color color = default)
		{
			_text.text = message;
			_text.color = color == default ? Color.white : color;

			_tween?.Kill();
			gameObject.SetActive(true);

			_canvasGroup.alpha = 0f;

			_tween = DOTween.Sequence()
				.Append(_canvasGroup.DOFade(1f, _fadeDuration))
				.AppendInterval(_lifeTime)
				.OnComplete(() => { onComplete?.Invoke(); });
		}

		public void Hide(Action onComplete)
		{
			_tween?.Kill();
			_tween = _canvasGroup.DOFade(0f, 0.5f)
				.OnComplete(() => { gameObject.SetActive(false); onComplete?.Invoke(); });
		}
	}
}