using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Scene.Fight;
using UnityEngine.UIElements;
using DG.Tweening;

public static class Extensions
{
	public static T Copy<T>(this T source, Transform parent = null) where T : Component
	{
		if (source == null)
		{
			Debug.LogError("Null prefab");
			return null;
		}

		return source.gameObject.Copy(parent).GetComponent(source.GetType()) as T;
	}

	public static GameObject Copy(this GameObject source, Transform parent = null)
	{
		if (source == null)
		{
			Debug.LogError("Null prefab");
			return null;
		}
		var copy = UnityEngine.Object.Instantiate(source, parent);
		copy.name = source.name;
		copy.SetActive(true);
		foreach (var component in copy.GetComponents<Component>())
		{
			if (component is Transform) continue;
			if (component is RectTransform) continue;
			component.gameObject.SetActive(true);
		}
		return copy;
	}
	public static IEnumerable<T> GetRandomUniqueElements<T>(this IEnumerable<T> source, int count, bool throwException = true)
	{
		if (source == null)
		{
			if (throwException)
				throw new InvalidOperationException("Cannot select random elements from a null collection");
			return Enumerable.Empty<T>();
		}

		var list = source.ToList();
		if (!list.Any())
		{
			if (throwException)
				throw new InvalidOperationException("Cannot select random elements from an empty collection");
			return Enumerable.Empty<T>();
		}

		if (count < 0)
			throw new ArgumentOutOfRangeException("Count cannot be negative");

		if (count > list.Count)
		{
			if (throwException)
				throw new InvalidOperationException($"Requested {count} elements but only {list.Count} available");
			count = list.Count;
		}

		var random = new System.Random();
		for (int i = 0; i < count; i++)
		{
			var j = random.Next(i, list.Count);

			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}

		return list.Take(count);
	}

	public static void Shuffle<T>(this IList<T> list, bool avoidOriginalNeighbors = true, int maxAttempts = 100)
	{
		int n = list.Count;

		if (!avoidOriginalNeighbors || n < 4)
		{
			RandomizeSimple(list);
			return;
		}

		var original = list.ToArray();

		for (int attempt = 0; attempt < maxAttempts; attempt++)
		{
			RandomizeSimple(list);
			if (!HasAdjacentOriginalNeighbors(list, original))
				return;
		}
	}

	private static void RandomizeSimple<T>(IList<T> list)
	{
		int n = list.Count;
		for (int i = 0; i < n - 1; i++)
		{
			int j = UnityEngine.Random.Range(i, n);
			T tmp = list[i];
			list[i] = list[j];
			list[j] = tmp;
		}
	}

	public static List<T> Multiply<T>(this IEnumerable<T> list, int times)
	{
		if (list == null || times <= 1)
			return new List<T>();
		var result = new List<T>(list.Count() * times);
		for (int i = 0; i < times; i++)
		{
			result.AddRange(list);
		}
		return result;
	}

	private static bool HasAdjacentOriginalNeighbors<T>(IList<T> shuffled, T[] original)
	{
		var indexMap = new Dictionary<T, int>(EqualityComparer<T>.Default);
		for (int i = 0; i < original.Length; i++)
			indexMap[original[i]] = i;

		for (int i = 0; i < shuffled.Count - 1; i++)
		{
			int idxA = indexMap[shuffled[i]];
			int idxB = indexMap[shuffled[i + 1]];
			if (Math.Abs(idxA - idxB) == 1)
				return true;
		}

		return false;
	}

	public static Vector2 GetDirection(this Transform transform, Transform target)
	{
		return GetDirection(transform.position, target.position);
	}

	public static Vector2 GetDirection(this Transform transform, Vector2 target)
	{
		return GetDirection(transform.position, target);
	}

	public static Vector2 GetDirection(this Vector2 position, Vector2 target)
	{
		return (target - position).normalized;
	}

	public static bool HasAll<T>(this T self, T flags)
		where T : Enum
	{
		long selfValue = Convert.ToInt64(self);
		long flagsValue = Convert.ToInt64(flags);
		return (selfValue & flagsValue) == flagsValue;
	}

	public static bool HasAnyCommon<T>(this T a, T b)
		where T : Enum
	{
		long aValue = Convert.ToInt64(a);
		long bValue = Convert.ToInt64(b);
		return (aValue & bValue) != 0;
	}

	public static void SetListener(this UnityEngine.UI.Button button, Action action)
	{
		if (button.IsNullUniversal()) return;

		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(() => action?.Invoke());
	}

	public static bool IsNullUniversal<T>(this T instance)
	{
		if (instance is UnityEngine.Object unityObject)
			return unityObject == null;

		return instance == null;
	}

	public static T GetRandomElement<T>(this IEnumerable<T> source, bool throwException = true)
	{
		if (source == null || !source.Any())
		{
			if (throwException)
			{
				throw new InvalidOperationException("Cannot select a random element from an empty or null collection.");
			}

			return default;
		}

		var random = new System.Random();

		var index = random.Next(0, source.Count());
		return source.ElementAt(index);
	}

	/// <summary>
	/// Анимирует появление target из положения 3D-объекта (Transform), разворачиваясь до финального положения.
	/// </summary>
	/// <param name="target">RectTransform окна</param>
	/// <param name="sourceWorldTransform">Transform объекта на сцене, из которого "выезжает" окно</param>
	/// <param name="camera">Камера, в которой виден объект. Если null — Camera.main</param>
	/// <param name="duration">Время анимации</param>
	/// <param name="easing">Тип easing</param>
	public static void AnimateExpandFromWorld(
		this RectTransform target,
		Transform sourceWorldTransform,
		Camera camera = null,
		float duration = 0.35f,
		Ease easing = Ease.OutCubic)
	{
		camera ??= Camera.main;

		// 1. Получаем позицию объекта на экране
		Vector3 screenPos = camera.WorldToScreenPoint(sourceWorldTransform.position);

		// 2. Переводим экранную позицию в локальные координаты канваса
		Canvas targetCanvas = target.GetComponentInParent<Canvas>();
		RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvasRect,
			screenPos,
			targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera,
			out Vector2 localPoint);

		// 3. Сохраняем финальную позицию
		Vector3 finalPosition = target.anchoredPosition3D;

		// 4. Устанавливаем стартовую позицию и scale
		target.anchoredPosition3D = localPoint;
		target.localScale = Vector3.zero;
		target.gameObject.SetActive(true);

		// 5. Анимируем
		Sequence seq = DOTween.Sequence();
		seq.Join(target.DOScale(1f, duration).SetEase(easing));
		seq.Join(target.DOAnchorPos3D(finalPosition, duration).SetEase(easing));
		// (опционально) альфа
		var cg = target.GetComponent<CanvasGroup>();
		if (cg != null)
		{
			cg.alpha = 0f;
			seq.Join(cg.DOFade(1f, duration * 0.6f));
		}
	}
}
