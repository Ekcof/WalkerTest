using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class RespawnPopup : MonoBehaviour, IPopup
{
	[SerializeField] private Button _respawnButton;
	[SerializeField] private Button _restartButton;

	public bool IsOpen => gameObject.activeSelf;

	public IPopup Close()
	{
		throw new System.NotImplementedException();
	}

	public IPopup Open()
	{
		throw new System.NotImplementedException();
	}
}
