using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Character
{
    public class ShootingState : TargetedState
    {
		protected override string LeftKey => "ShootLeft";

		protected override string RightKey => "ShootRight";

		public override void Start()
		{
			SetAnimationFromSide(CurrentSide);
		}

		public override void Update()
		{
			base.Update();

			SetAnimation();
		}
	}
}