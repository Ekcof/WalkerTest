using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface IPopup
    {
		bool IsOpen { get; }
		IPopup Open();
		IPopup Close();
	}
}
