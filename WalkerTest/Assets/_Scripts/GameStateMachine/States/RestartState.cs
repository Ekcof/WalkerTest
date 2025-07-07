using Cysharp.Threading.Tasks;
using Serialization;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gamestates
{
	public class RestartState : State
	{
		[Inject] private ISerializationManager _serialization;
		public override GameStateType StateType => GameStateType.RestartState;

		public override void Start()
		{
			LoadSceneAsync(SceneManager.GetActiveScene().name).Forget();
		}

		private async UniTask LoadSceneAsync(string sceneName)
		{
			try
			{
				await _serialization.DeleteProgress();
			}
			catch
			{
				return;
			}

			var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
			while (!asyncOperation.isDone)
			{
				try
				{
					await UniTask.DelayFrame(1);
				}
				catch
				{
					break;
				}
				await UniTask.Yield();
			}
		}

		public override void Stop()
		{
		}
	}
}