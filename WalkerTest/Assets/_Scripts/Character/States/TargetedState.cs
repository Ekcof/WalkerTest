using Scene.Detection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Character
{
    public abstract class TargetedState : State
    {
        protected ITarget CurrentTarget;
		protected bool IsRight;
		public void SetTarget(ITarget target)
		{
			CurrentTarget = target;
		}

		protected virtual void SetAnimForSide()
		{
			var currentSide = CurrentTarget.Character.Position.x > _root.Position.x;

			if (IsRight != currentSide)
			{
				var animName = currentSide ?
					"MoveRight" : "MoveLeft";
				_root.Animator?.SetAnimation(animName);
				IsRight = currentSide;
			}
		}
	}
}