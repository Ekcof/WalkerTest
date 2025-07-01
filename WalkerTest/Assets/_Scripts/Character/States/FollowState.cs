using Scene.Detection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Character
{
	public class FollowState : State
	{
		public ITarget Target { get; private set; }
		private bool _isRight;

		public void SetTarget(ITarget target)
		{
			Target = target;
		}

		public override void Start()
		{
			_root.Movement?.Follow(Target.Character.Transform);

			SetAnimForSide();
		}

		public override void Update()
		{
			base.Update();

			SetAnimForSide();
		}

		private void SetAnimForSide()
		{
			var currentSide = Target.Character.Position.x > _root.Position.x;

			if (_isRight != currentSide)
			{
				var animName = currentSide ?
					"MoveRight" : "MoveLeft";
				_root.Animator?.SetAnimation(animName);
				_isRight = currentSide;
			}
		}
	}
}
