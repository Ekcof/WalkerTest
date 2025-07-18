using Scene.Detection;
using System;

namespace Scene.Character
{
	public abstract class TargetedState : State
	{
		protected bool IsRight;
		protected abstract string LeftKey { get; }
		protected abstract string RightKey { get; }
		public ITarget Target { get; private set; }

		public bool CurrentSide => Target != null ? Target.Character.Position.x > _root.Position.x
			: _root.Movement.CurrentDirection.x >= 0;

		public void Start(ITarget target)
		{
			Target = target;
			Start();
		}

		protected virtual void SetAnimation(Action onComplete = null)
		{

			var currentSide = Target == null ? _root.Movement.CurrentDirection.x > 0 :
				Target.Character.Position.x > _root.Position.x;

			if (IsRight != currentSide)
			{
				IsRight = currentSide;
				SetAnimationFromSide(IsRight, onComplete);
			}
		}

		protected void SetAnimationFromSide(bool isRight, Action onComplete = null)
		{
			var animName = isRight ?
	RightKey : LeftKey;
			_root.Animator?.SetAnimation(animName, onComplete);
		}
	}
}