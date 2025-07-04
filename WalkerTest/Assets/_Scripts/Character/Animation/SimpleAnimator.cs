using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Scene.Character
{
	/// <summary>
	/// Stop motion animation data structure
	/// </summary>
	[Serializable]
	public class Animation
	{
		public string Id;
		public int FPS = 8;
		public bool Loop = true;
		public Frame[] Frames;
	}

	[Serializable]
	public struct Frame
	{
		public Sprite Sprite;
		public bool FlipX;
		public bool FlipY;
	}

	/// <summary>
	/// SimpleAnimator is a basic animator that plays stop motion
	/// </summary>
	public class SimpleAnimator : MonoBehaviour, IAnimator
	{
		[SerializeField] private Animation[] _animations;
		[SerializeField] private SpriteRenderer _image;

		private Animation _defaultAnimation;
		private Sequence _sequence;

		public void SetDefaultAnimation()
		{
			if (_defaultAnimation != null)
			{
				SetAnimation(_defaultAnimation.Id);
			}
			else if (_animations.Length > 0)
			{
				_defaultAnimation = _animations[0];
				SetAnimation(_defaultAnimation.Id);
			}
		}

		public void SetAnimation(string id, Action onComplete = null)
		{
			if (_sequence != null && _sequence.IsActive())
			{
				_sequence.Kill();
			}
			var animation = _animations.FirstOrDefault(a => a.Id == id);
			if (animation == null || animation.Frames == null || animation.Frames.Length == 0) return;

			_sequence = DOTween.Sequence();
			float frameDuration = 1f / animation.FPS;

			foreach (var frame in animation.Frames)
			{
				_sequence.AppendCallback(() =>
				{
					_image.sprite = frame.Sprite;
					_image.flipX = frame.FlipX;
					_image.flipY = frame.FlipY;
				});
				_sequence.AppendInterval(frameDuration);
			}

			if (animation.Loop)
			{
				_sequence.SetLoops(-1);
			}

			if (onComplete != null)
			{
				_sequence.OnStepComplete(() =>
				{
					onComplete.Invoke();
				});
			}
			Play();
		}

		public void Pause()
		{
			_sequence?.Pause();
		}

		public void Play()
		{
			if (_sequence == null)
			{
				SetDefaultAnimation();
			}
			_sequence?.Play();
		}
	}
}
