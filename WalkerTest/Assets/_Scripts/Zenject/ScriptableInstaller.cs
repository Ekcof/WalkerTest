
using Inventory;
using Scene.Fight;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Configs Installer", menuName = "Installers/Configs Installer")]
public class ScriptableInstaller : ScriptableObjectInstaller
{
	[SerializeField] private BulletConfigHolder _bulletConfigHolder;
	[SerializeField] private ItemConfigHolder _itemConfigHolder;
	public override void InstallBindings()
	{
		Bind(_bulletConfigHolder);
		Bind(_itemConfigHolder);
	}

	private void Bind<T>(T instance) where T : ScriptableObject
	{
		if (typeof(T).GetInterfaces().Any())
		{
			Container.BindInterfacesAndSelfTo<T>().FromInstance(instance);
			return;
		}

		Container.BindInstance(instance);
	}
}
