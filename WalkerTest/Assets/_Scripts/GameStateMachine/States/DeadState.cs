

namespace Gamestates
{
    public class DeadState : State
    {
		public override GameStateType StateType => GameStateType.DeadState;

		public override void Start()
		{
			throw new System.NotImplementedException();
		}

		public override void Stop()
		{
			throw new System.NotImplementedException();
		}
    }
}