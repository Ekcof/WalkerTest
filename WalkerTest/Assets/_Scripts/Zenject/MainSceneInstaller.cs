using Controller;
using Scene.Character;
using Scene.Fight;
using UnityEngine;


public class MainSceneInstaller : BaseInstaller
{
	[SerializeField] private Camera _camera;
	[SerializeField] private JoyStick _joyStick;
	[SerializeField] private Player _player; // TODO: Register players by ID
	[Header("Registries")]
	[SerializeField] private CharacterRegistry _characterRegistry;
	[SerializeField] private BulletRegistry _bulletRegistry;
	public override void InstallBindings()
	{
		Container.BindInstance(_camera).WithId("mainCam");

		Bind(_joyStick);
		Bind(_player);

		Bind(_characterRegistry);
		Bind(_bulletRegistry);
		Bind<ShootableRegisty>();
	}
}