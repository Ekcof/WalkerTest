using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Scene.Character;
using System.Linq;

namespace Scene.Detection
{

	public interface IDetector
	{
		IReadOnlyReactiveCollection<ITarget> Targets { get; }
		bool HasTargets { get; }
		ITarget NearestTarget { get; }
		void ToggleDetection(bool isActive);
	}

	public class Detector : MonoBehaviour, IDetector
	{
		[Inject] private ICharacterRegistry _characterRegistry;

		[SerializeField] private float _detectionRadius = 5f;

		private CancellationTokenSource _detectionCts;
		private bool IsDetecting => _detectionCts != null && !_detectionCts.IsCancellationRequested;
		public IReadOnlyReactiveCollection<ITarget> Targets => throw new System.NotImplementedException();

		public bool HasTargets => Targets.Count > 0;

		public ITarget NearestTarget
		{
			get
			{
				if (Targets.Count == 0) return null;

				return Targets.OrderBy(target => Vector3.Distance(target.Character.Transform.position, transform.position)).FirstOrDefault();
			}
		}

		public void ToggleDetection(bool isActive)
		{
			if (!isActive)
			{
				_detectionCts?.CancelAndDispose();
				_detectionCts = null;
			}
			else if (!IsDetecting)
			{
				_detectionCts = new();
				StartDetection(_detectionCts.Token).Forget();
			}
		}

		private async UniTask StartDetection(CancellationToken token)
		{
			while (_detectionCts != null && !token.IsCancellationRequested)
			{
				// Perform detection logic here
				try
				{
					await UniTask.Yield(token);
				}
				catch
				{
					return;
				}
			}
		}

		private void OnDestroy()
		{
			_detectionCts?.CancelAndDispose();
		}
	}
}
