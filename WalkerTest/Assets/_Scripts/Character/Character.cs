using Inventory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Scene.Character
{
    public interface ICharacter
    {
		string ID {  get; }
		string Name { get; }
		Transform Transform { get; }
		Vector3 Position { get; }
		Health Health { get; }
        IMovement Movement { get; }
        IInventory Inventory { get; }
		IState State { get; }
		IAnimator Animator { get; }
		float CurrentDamage { get; }
	}

	public abstract class Character : MonoBehaviour, ICharacter
	{
		[SerializeField] private string _id;
		[SerializeField] private Movement _movement;
		[SerializeField] private MainInventory _inventory;
		[SerializeField] private Health _health;
		[SerializeField] private SimpleAnimator _animator;
		protected IState CurrentState;

		public string ID { get; private set; }
		public string Name { get; private set; }
		public Transform Transform => transform;
		public Vector3 Position => transform.position;
		public IState State => CurrentState;
		public IAnimator Animator => _animator;
		public IMovement Movement => _movement;
		public IInventory Inventory => _inventory;
		public Health Health => _health;
		public abstract float CurrentDamage { get; }

		protected virtual void LateUpdate()
		{
			CurrentState?.Update();
		}

		protected void SetState(IState state)
		{
			CurrentState?.Stop();
			CurrentState = state;
			CurrentState?.Start();
		}
	}
}