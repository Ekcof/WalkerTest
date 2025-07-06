using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gamestates
{
	public enum GameStateType
	{
		LoadingState,
		MainState,
		DeadState,
		RestartState,
		RespawnState
	}

	public interface IState
	{
		GameStateType StateType { get; }
		void Start();
		void Stop();
	}

	public abstract class State : IState
	{
		[Inject] protected DiContainer _container;
		[Inject] protected IGameStateMachine StateMachine;
		public abstract GameStateType StateType { get; }

		public abstract void Start();
		public abstract void Stop(); 
	}
}