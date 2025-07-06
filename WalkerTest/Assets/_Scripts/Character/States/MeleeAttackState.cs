namespace Scene.Character
{
    public class MeleeAttackState : TargetedState
	{
		protected override string LeftKey => "MeleeAttackLeft";

		protected override string RightKey => "MeleeAttackRight";

		public override void Start()
		{
			_root.Movement?.Stop();
			SetAnimationFromSide(CurrentSide, OnAnimationComplete);
		}

		public override void Update()
		{
			base.Update();

			SetAnimation(OnAnimationComplete);
		}

		private void OnAnimationComplete()
		{
			UnityEngine.Debug.Log($"_______!Try to hit");
			if (Target != null)
				UnityEngine.Debug.Log($"_______! Hit {Target.Character.Name}");
			Target?.Character.Health.TryApplyChange(-_root.CurrentDamage);
		}
	}
}