using UniRx;

public interface IValueObserver
{
	float MaxValue { get; }
	IReadOnlyReactiveProperty<float> Current { get; }
	bool TryApplyChange(float amount);
	void Refresh();
	void UpdateMaxValue(float amount);
}