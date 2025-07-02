using Controller;
using Scene.Character;
using UnityEngine;


public class MainSceneInstaller : BaseInstaller
{
	[SerializeField] private Camera _camera;
	[SerializeField] private JoyStick _joyStick;
	[SerializeField] private CharacterRegistry _characterRegistry;


	public override void InstallBindings()
	{
		Container.BindInstance(_camera).WithId("mainCam");
		Bind(_characterRegistry);
		Bind(_joyStick);
	}
}