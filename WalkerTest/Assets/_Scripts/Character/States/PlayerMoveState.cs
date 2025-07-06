using Scene.Character;
using Scene.Fight;

public class PlayerMoveState : TargetedState
{
	protected override string LeftKey => "MoveLeft";

	protected override string RightKey => "MoveRight";

	public override void Start()
	{
		SetAnimationFromSide(CurrentSide);
	}

	public void SetWeapon(IGun gun)
	{

	}

	public override void Update()
	{
		base.Update();

		SetAnimation();
	}
}
