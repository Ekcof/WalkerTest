using Scene.Character;
using System;
using UnityEngine;
using Zenject;

namespace Inventory
{
    public interface IItem
    {
        string Id { get; }
        int Amount { get; }
        bool IsStackable { get; }
		int StackLimit { get; }
		int BasePrice { get; }
		Sprite Sprite { get; }
		bool TryToUse(ICharacter character); //
		int AddAmount(int amountToAdd);
		IItem Copy(DiContainer diContainer, int amount);
	}

	[Serializable]
    public abstract class Item : IItem
	{
		[SerializeField] private string _id;
		[SerializeField] private bool _isStackable;
		[SerializeField] private int _stackLimit;
		[SerializeField] private Sprite _sprite;
		[SerializeField] private int _basePrice;

		public string Id => _id;

		[NonSerialized] protected int AmountInternal;
		public int Amount => AmountInternal;
		
		public bool IsStackable => _isStackable;

		public int StackLimit => _stackLimit;

		public Sprite Sprite => _sprite;
		public int BasePrice => _basePrice;
		
		public int AddAmount(int amountToAdd)
		{
			AmountInternal = Math.Clamp(AmountInternal + amountToAdd, 0, int.MaxValue);
			return AmountInternal; // return amount left
		}

		public abstract bool TryToUse(ICharacter character);
		public IItem Copy(DiContainer diContainer, int amount)
		{
			var type = GetType();
			var newInstance = (Item)diContainer.Instantiate(type);
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(this), newInstance);
			newInstance.AmountInternal = amount;

			diContainer.Inject(newInstance);
			return newInstance;
		}

	}
}