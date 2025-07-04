using UniRx;

public interface IValueObserver
{
	float MaxValue { get; }
	IReadOnlyReactiveProperty<float> CurrentValue { get; }
	bool TryApplyChange(float amount);
	void Refresh();
	void UpdateMaxValue(float amount);
}