using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Character
{
	public interface IAnimator
	{
		void SetDefaultAnimation();
		void SetAnimation(string id, Action onComplete = null);
		void Play();
		void Pause();
	}
}