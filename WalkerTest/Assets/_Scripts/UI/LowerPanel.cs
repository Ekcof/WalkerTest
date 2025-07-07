using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UI.Controls;
using UnityEngine;

namespace UI
{
    public interface ILowerPanel
    {
		bool IsActive { get; }
        void Activate();
        void Deactivate();
    }

    public class LowerPanel : MonoBehaviour, ILowerPanel
	{
        [SerializeField] private CanvasGroup _canvasGroup;
		private Tween _cgTween;
		public bool IsActive => _canvasGroup.alpha == 1f;

		public void Activate()
		{
			_canvasGroup.alpha = 0f;
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
			_cgTween?.Kill();
			_cgTween = _canvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutBack);
		}

		public void Deactivate()
		{
			_canvasGroup.alpha = 1f;
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
			_cgTween?.Kill();
			_cgTween = _canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.OutBack);
		}
	}
}