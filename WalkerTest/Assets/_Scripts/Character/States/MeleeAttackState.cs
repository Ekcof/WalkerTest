using Scene.Detection;

namespace Scene.Character
{
    public class MeleeAttackState : State
    {
		public ITarget Target { get; private set; }
		private bool _isRight;

		public void SetTarget(ITarget target)
		{
			Target = target;
		}

		public override void Start()
		{
			SetAnimForSide();
		}

		public override void Update()
		{
			base.Update();

			SetAnimForSide();
		}

		private void SetAnimForSide()
		{
			void OnAnimationComplete()
			{
				Target.Character.Health?.TryApplyChange(1f);
			}

			var currentSide = Target.Character.Position.x > _root.Position.x;

			if (_isRight != currentSide)
			{
				var animName = currentSide ?
					"MeleeAttackRight" : "MeleeAttackLeft";
				_root.Animator?.SetAnimation(animName, OnAnimationComplete);
				_isRight = currentSide;
			}
		}
	}
}