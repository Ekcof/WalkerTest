using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene.Character
{
    public class DeadState : State
    {
		public override void Start()
		{
			_root.Movement.Stop();
			_root.Animator.SetAnimation("Die");
		}
    }
}