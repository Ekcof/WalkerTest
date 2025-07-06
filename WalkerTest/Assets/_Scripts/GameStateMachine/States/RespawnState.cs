
using Scene.Character;
using Zenject;

namespace Gamestates
{
    public class RespawnState : State
    {
		[Inject] private Player _player;

		public override GameStateType StateType => GameStateType.RespawnState;

		public override void Start()
		{
			_player.Respawn();
			StateMachine.TryChangeState(GameStateType.MainState);
		}

		public override void Stop()
		{
		}
    }
}
