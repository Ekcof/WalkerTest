using Gamestates;
using Inventory;
using Scene.Character;
using Scene.Fight;
using Serialization;
using UI.Controls;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class MainSceneInstaller : BaseInstaller
{
	[SerializeField] private Camera _camera;
	[SerializeField] private Light2D _globalLight;

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
		Container.BindInstance(_globalLight).WithId("globalLight");

		Bind(_joyStick);
		Bind(_player);
		Bind(_playerLog);

		Bind(_characterRegistry);
		Bind(_bulletRegistry);
		Bind(_itemHolderRegistry);
		Bind<ShootableRegisty>();
		Bind<GameStateMachine>();
		Bind<SerializationManager>();
	}
}