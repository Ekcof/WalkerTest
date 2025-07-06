

using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Gamestates
{
	public class DeadState : State
	{
		[Inject(Id = "globalLight")] private Light2D _light;
		[Inject] private RespawnPopup _respawnPopup;

		public override GameStateType StateType => GameStateType.DeadState;
		private Tween _tween;

		public override void Start()
		{
			_tween = DOTween.To(
				() => _light.color,
				c => _light.color = c,
				Color.red, 
				1f
			);
			_respawnPopup.Open();
		}

		public override void Stop()
		{
			_tween?.Kill();
		}
	}
}