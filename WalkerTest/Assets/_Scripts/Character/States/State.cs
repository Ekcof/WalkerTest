using System;
using System.Threading;

namespace Scene.Character
{
	public interface IState: IDisposable
	{
		bool IsInitialized { get; }
		void Initialize(ICharacter character);
		void Start();
		void Update();
		void Stop();
	}

	public abstract class State : IState
	{
		protected ICharacter _root;
		protected CancellationTokenSource Cts;
		public bool IsInitialized => _root != null;

		public virtual void Initialize(ICharacter character)
		{
			_root = character;
		}

		public abstract void Start();
		public virtual void Update()
		{
		}

		public virtual void Stop()
		{
			Cts?.CancelAndDispose();
		}

		public virtual void Dispose()
		{
			Cts?.CancelAndDispose();
		}
	}
}
