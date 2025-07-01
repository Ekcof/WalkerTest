using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class Health : MonoBehaviour, IValueObserver
{
	[SerializeField] private float _maxHealth = 100f;
	private ReactiveProperty<float> _currentHealth = new();

	public float MaxValue => _maxHealth;

	public IReadOnlyReactiveProperty<float> CurrentValue => _currentHealth;

	public void ApplyChange(float amount)
	{
		throw new System.NotImplementedException();
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
