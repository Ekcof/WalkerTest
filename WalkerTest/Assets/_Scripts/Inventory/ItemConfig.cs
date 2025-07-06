using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Inventory
{
    public interface IItemConfig
    {
		string ID { get; }
        IItem GetCopy(DiContainer diContainer, int amount);
    }


	[Serializable]
	public abstract class ItemConfig : ScriptableObject, IItemConfig
	{
		public abstract string ID { get; }
		public abstract IItem GetCopy(DiContainer diContainer, int amount);
	}

	public abstract class ItemConfig<T>: ItemConfig where T : Item
    {
        [SerializeField] private T _data;
		public override string ID => _data.Id;

		public override IItem GetCopy(DiContainer diContainer, int amount)
		{
			return _data.Copy(diContainer, amount);
		}
	}
}