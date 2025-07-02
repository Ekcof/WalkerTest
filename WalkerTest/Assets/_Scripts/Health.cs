using UniRx;
using UnityEngine;


public class Health : MonoBehaviour, IValueObserver
{
	[SerializeField] private float _maxHealth = 100f;
	private ReactiveProperty<float> _currentHealth = new(100f);

	public float MaxValue => _maxHealth;

	public IReadOnlyReactiveProperty<float> CurrentValue => _currentHealth;

	public void TryApplyChange(float amount)
	{
		if (_currentHealth.Value > 0)
		{
			_currentHealth.Value += amount;
		}
		Debug.Log($"____Try apply {amount} change {CurrentValue}");

		//throw new System.NotImplementedException();
	}

	public void Refresh()
	{
		_currentHealth.Value = MaxValue;
	}

	public void UpdateMaxValue(float amount)
	{
		_maxHealth = amount;
		_currentHealth.Value = Mathf.Min(_currentHealth.Value, _maxHealth);
	}
}
