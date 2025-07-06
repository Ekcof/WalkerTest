
using Scene.Character;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Serialization;

namespace Gamestates
{
	public class MainState : State
	{
		private const int SAVE_PERIOD = 10;
		[Inject] private Player _player;
		[Inject] private IGameStateMachine _gameStateMachine;
		[Inject] private ISerializationManager _serializationManager;

		[Inject(Id = "globalLight")] private Light2D _light;
		private CompositeDisposable _compositeDisposable = new();
		private CancellationTokenSource _cts;
		public override GameStateType StateType => GameStateType.MainState;

		public override void Start()
		{
			_player.Health.Current.Subscribe(OnHealthChanged).AddTo(_compositeDisposable);
			_cts?.CancelAndDispose();
			_cts = new();

			if (_light.color != Color.white)
			{
				_light.color = Color.white;
			}

			PeriodicalSaver(_cts.Token).Forget();
		}

		private void OnHealthChanged(float health)
		{
			if (health <= 0)
			{
				_gameStateMachine.TryChangeState(GameStateType.DeadState);
			}
		}

		public override void Stop()
		{
			_compositeDisposable.Clear();
		}

		private async UniTask PeriodicalSaver(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(SAVE_PERIOD), cancellationToken: token);
				if (!token.IsCancellationRequested)
				{
					Debug.Log($"____Try to save");
					await _serializationManager.TryToSaveAsync(token);
				}
			}
		}
	}
}