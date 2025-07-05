using Controller;
using Inventory;
using Scene.Character;
using Scene.Fight;
using UnityEngine;


public class MainSceneInstaller : BaseInstaller
{
	[SerializeField] private Camera _camera;
	[SerializeField] private JoyStick _joyStick;
	[SerializeField] private Player _player; // TODO: Register players by ID
	[SerializeField] private UI.PlayerLog _playerLog;
	[Header("Registries")]
	[SerializeField] private CharacterRegistry _characterRegistry;
	[SerializeField] private BulletRegistry _bulletRegistry;
	[SerializeField] private ItemHolderRegistry _itemHolderRegistry;

	public override void InstallBindings()
	{
		Container.BindInstance(_camera).WithId("mainCam");

		Bind(_joyStick);
		Bind(_player);
		Bind(_playerLog);

		Bind(_characterRegistry);
		Bind(_bulletRegistry);
		Bind(_itemHolderRegistry);
		Bind<ShootableRegisty>();
	}
}