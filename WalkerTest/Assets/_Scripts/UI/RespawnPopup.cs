using DG.Tweening;
using Gamestates;
using Inventory;
using Scene.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class RespawnPopup : MonoBehaviour, IPopup
	{
		[Inject(Id ="mainCam")] private Camera _camera;
		[Inject] private IGameStateMachine _stateMachine;
		[Inject] private InventoryPopup _inventoryPopup;
		[Inject] private LowerPanel _lowerPanel;
		[Inject] private Player _player;

		[SerializeField] private Button _respawnButton;
		[SerializeField] private Button _restartButton;

		public bool IsOpen => gameObject.activeSelf;

		private void Awake()
		{
			_respawnButton.SetListener(OnRespawnButtonClicked);
			_restartButton.SetListener(OnRestartButtonClicked);
		}

		private void OnRestartButtonClicked()
		{
			_stateMachine.TryChangeState(GameStateType.RestartState);
			Close();
		}

		private void OnRespawnButtonClicked()
		{
			_stateMachine.TryChangeState(GameStateType.RespawnState);
			Close();
		}

		public IPopup Open()
		{
			var windowRect = (RectTransform)transform;
			gameObject.SetActive(true);
			windowRect.AnimateExpandFromWorld(_player.transform, Camera.main, 0.45f, Ease.OutBack);
			return this;
		}

		public IPopup Close()
		{
			gameObject.SetActive(false);
			return this;
		}
	}
}