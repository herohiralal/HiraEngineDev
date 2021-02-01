using UnityEngine;

public class AIComponent : MonoBehaviour
{
	[SerializeField] private BlackboardComponent blackboardComponent = null;

	private void Reset()
	{
		if (blackboardComponent == null) blackboardComponent = GetComponent<BlackboardComponent>();
	}

	private void OnValidate()
	{
		if (blackboardComponent == null) blackboardComponent = GetComponent<BlackboardComponent>();
	}

	private void Awake()
	{
		blackboardComponent.OnGoalUpdate += OnGoalUpdate;
	}

	private static void OnGoalUpdate(int archetypeIndex)
	{
		Debug.Log($"Goal updated to: {archetypeIndex}.");
	}

	private void OnDestroy()
	{
		blackboardComponent.OnGoalUpdate -= OnGoalUpdate;
	}
}