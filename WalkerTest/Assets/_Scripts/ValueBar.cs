using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scene.UI
{
	public interface IValueBar
	{
		void ApplyValue(IValueObserver valueObserver);

		void SetVisible(bool visible);
	}

	public class ValueBar : MonoBehaviour, IValueBar
	{
		[SerializeField] private Image _bar;
		[SerializeField]
		private Gradient _gradient = new Gradient
		{
			colorKeys = new[]
	{
		new GradientColorKey(Color.red, 0f),
		new GradientColorKey(Color.green, 1f)
	}
		};
		private float _maxValue;

		public void ApplyValue(IValueObserver valueObserver)
		{
			_maxValue = valueObserver.MaxValue;
			valueObserver.CurrentValue.Subscribe(OnChangeValue).AddTo(this);
		}

		private void OnChangeValue(float obj)
		{
			float fill = obj / _maxValue;
			_bar.fillAmount = fill;

				_bar.color = _gradient.Evaluate(fill);

			gameObject.SetActive(fill > 0f);
		}

		public void SetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}
	}
}