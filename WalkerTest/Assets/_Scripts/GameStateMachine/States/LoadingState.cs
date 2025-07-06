
using Cysharp.Threading.Tasks;
using Serialization;
using System.Diagnostics;
using System.Threading;
using UI;
using Zenject;

namespace Gamestates
{
	public class LoadingState : State
	{
		[Inject] private ISerializationManager _serialization;
		[Inject] private IGameStateMachine _gameStateMachine;
		[Inject] private IPlayerLog _playerLog;

		public override GameStateType StateType => GameStateType.LoadingState;
		private CancellationTokenSource _cts;

		public override void Start()
		{
			_cts?.CancelAndDispose();
			_cts = new();

			Deserialize(_cts.Token).Forget();
		}

		private async UniTask Deserialize(CancellationToken token)
		{
			bool hasProgress;
			try
			{
				hasProgress = await _serialization.TryToLoadAsync(token);
			}
			catch
			{
				UnityEngine.Debug.LogError("Failed to load player data.");
				return;
			}

			_playerLog.AddMessage(hasProgress ? "Progress has been loaded" : "No progress found");

			if (_gameStateMachine.TryChangeState(GameStateType.MainState))
			{
				_playerLog.AddMessage("Game has been successfully started.");
			}
			else
			{
				_playerLog.AddMessage("Failed to change to MainState.");
			}
		}


		public override void Stop()
		{
			_cts?.CancelAndDispose();
		}
	}
}