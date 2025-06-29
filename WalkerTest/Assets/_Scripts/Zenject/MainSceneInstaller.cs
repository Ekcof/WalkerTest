using Controller;
using UnityEngine;


	public class MainSceneInstaller : BaseInstaller
	{
		[SerializeField] private Camera _camera;
	[SerializeField] private JoyStick _joyStick;


		public override void InstallBindings()
		{
			Container.BindInstance(_camera).WithId("mainCam");

			Bind(_joyStick);
		}
	}