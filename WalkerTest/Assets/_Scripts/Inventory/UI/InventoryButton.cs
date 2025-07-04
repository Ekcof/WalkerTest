using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{

    [SerializeField] private InventoryPopup _popup;
    [SerializeField] private Button _button;

	void Start()
    {
        _button.SetListener(OnPressButton);
    }

	private void OnPressButton()
	{
		if (_popup.IsOpen)
		{
			_popup.Close();
		}
		else
		{
			_popup.Open();
		}
	}
}
