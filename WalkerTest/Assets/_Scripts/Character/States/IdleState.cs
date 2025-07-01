
namespace Scene.Character
{
    public class IdleState : State
    {
		public override void Start()
		{
			_root.Movement?.Stop();
			_root.Animator?.SetAnimation("Idle");
		}
    }
}