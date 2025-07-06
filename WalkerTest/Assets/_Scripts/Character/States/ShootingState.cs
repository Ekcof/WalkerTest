using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Character
{
    public class ShootingState : TargetedState
    {
		protected override string LeftKey => "MoveLeft";

		protected override string RightKey => "MoveRight";

		public override void Start()
		{
			_root.Movement.Follow(Target.Transform);
			SetAnimationFromSide(CurrentSide);
		}

		public override void Update()
		{
			base.Update();

			SetAnimation();
		}
	}
}