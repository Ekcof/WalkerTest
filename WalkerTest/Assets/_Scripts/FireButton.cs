using Scene.Character;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FireButton : MonoBehaviour
{
	[Inject] private Player _player;
 	[SerializeField] private Button _button;

	private void Awake()
	{
		_button.SetListener(OnFireButtonClicked);
	}

	private void OnFireButtonClicked()
	{
		_player.TryToShoot();
	}
}
