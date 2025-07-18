using Inventory;
using Scene.Detection;
using Scene.Fight;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Scene.Character
{
	public interface ICharacter: IShootable
	{
		string ID { get; }
		string Name { get; }
		Transform Transform { get; }
		Vector3 Position { get; }
		IMovement Movement { get; }
		IInventory Inventory { get; }
		IState State { get; }
		IAnimator Animator { get; }
		float CurrentDamage { get; }
		bool IsDead { get; }
		float TimeOfDeath { get; }

		void DropLoot();
		void Refresh();
	}

	public abstract class Character : MonoBehaviour, ICharacter
	{
		[Inject] private IShootableRegistry _shootables;
		[Inject] private ICharacterRegistry _characters;
		[Inject] private IItemHolderRegistry _itemHolders;

		[SerializeField] private string _id;
		[SerializeField] private Movement _movement;
		[SerializeField] private MainInventory _inventory;
		[SerializeField] private Health _health;
		[SerializeField] private SimpleAnimator _animator;
		[SerializeField] protected string _name;
		[SerializeField] protected Collider2D _collider;
		[SerializeField] protected TargetType _targetType;
		[SerializeField] protected bool CanDropLoot = true;

		protected IState CurrentState;
		protected ITarget _currentTarget;

		public string ID { get; private set; }
		public string Name { get; private set; }
		public Transform Transform => transform;
		public Vector3 Position => transform.position;
		public string Hash => $"{_id}_{transform.GetHashCode()}";
		public IState State => CurrentState;
		public IAnimator Animator => _animator;
		public IMovement Movement => _movement;
		public IInventory Inventory => _inventory;
		public Health Health => _health;
		public Collider2D Collider => _collider;
		public abstract float CurrentDamage { get; }
		public bool IsDead => Health.Current.Value <= 0;
		public TargetType TargetType => _targetType;
		public float TimeOfDeath { get; protected set; }

		protected virtual void Start()
		{
			Refresh();
		}

		public virtual void Refresh()
		{
			CurrentState = null;
			TimeOfDeath = 0;
			Health?.Refresh();
			_collider.enabled = true;
			gameObject.SetActive(true);
			Register();
		}

		protected virtual void Register()
		{
			_shootables.Register(this);
			_characters.Register(this);
		}

		public void Unregister()
		{
			_shootables.Unregister(this);
			_characters.Unregister(this);
		}

		protected virtual void LateUpdate()
		{
			CurrentState?.Update();
		}

		protected void SetState(IState state)
		{
			CurrentState?.Stop();

			if (!state.IsInitialized)
			{
				state.Initialize(this);
			}

			CurrentState = state;
			if (State is TargetedState targeted)
			{
				targeted.Start(_currentTarget);
			}
			else
			{
				CurrentState?.Start();
			}
		}

		public void DropLoot()
		{
			if (!CanDropLoot || Inventory.AllItems.Count() == 0)
			{
				return;
			}

			_itemHolders.Create(Inventory.AllItems, transform.position);
		}
	}
}