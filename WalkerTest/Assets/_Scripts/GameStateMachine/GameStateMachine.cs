using Scene.Character;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Zenject;

namespace Gamestates
{
	public interface IGameStateMachine : IInitializable
	{
		GameStateType CurrentStateType { get; }
		IReadOnlyReactiveProperty<IState> CurrentState { get; }
		bool TryChangeState(GameStateType type);
	}


	public class GameStateMachine : IGameStateMachine
	{
		[Inject] private DiContainer _diContainer;
		private ReactiveProperty<IState> _currentState = new();

		private HashSet<IState> _states = new()
		{
			new LoadingState(),
			new MainState(),
			new DeadState(),
			new RespawnState(),
			new RestartState()
		};

		public GameStateType CurrentStateType => _currentState.Value.StateType;
		public IReadOnlyReactiveProperty<IState> CurrentState => throw new System.NotImplementedException();

		public void Initialize()
		{
			foreach (var state in _states)
			{
				_diContainer.Inject(state);
			}

			TryChangeState(GameStateType.LoadingState);
		}

		public bool TryChangeState(GameStateType type)
		{
			if (_currentState.Value?.StateType == type)
			{
				return false;
			}

			_currentState.Value?.Stop();

			var state = _states.FirstOrDefault(s => s.StateType == type);

			if (state != null)
			{
				_currentState.Value = state;
				state?.Start();
			}

			return state != null;
		}


	}
}
