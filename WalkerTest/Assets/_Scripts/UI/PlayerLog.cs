using ComponentUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
	public interface IPlayerLog
	{
		void AddMessage(string message, Color color = default);
		void ClearLog();
	}

	public class PlayerLog : MonoBehaviour, IPlayerLog
	{
		[Inject] private DiContainer _diContainer;
		[SerializeField] private PlayerLogSlot _slotPrefab;
		[SerializeField] private int _logsLimit = 3;

		private Pool<PlayerLogSlot> _pool;
		private Queue<PlayerLogSlot> _slots = new();

		private void Awake()
		{
			_pool = new(_slotPrefab, transform, _diContainer);
		}

		public void AddMessage(string message, Color color = default)
		{
			if (_slots.Count > _logsLimit)
			{
				HideLastSlot();
			}
			
			var slot = _pool.Pop();
			slot.Show(message, HideLastSlot, color);
			_slots.Enqueue(slot);
			//LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
		}

		public void ClearLog()
		{
			foreach (var slot in _slots)
			{
				HideLastSlot();
			}
			_slots.Clear();
		}

		private void HideLastSlot()
		{
			var slot = _slots.Dequeue();
			slot.Hide(() => _pool.Push(slot));
		}
	}
}