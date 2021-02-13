using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class AIComponent : MonoBehaviour
{
	[SerializeField] private BlackboardComponent blackboardComponent = null;
	[SerializeField] private byte maxPlanLength = 15;
	[SerializeField] private float maxFScore = 100;

	private List<IActualAction> _actions = null;
	private ActualPlanStack _planStack = null;

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
		_actions = new List<IActualAction>();
		_planStack = new ActualPlanStack(maxPlanLength);
		blackboardComponent.OnGoalUpdate += OnGoalUpdate;
	}

	private void OnGoalUpdate(int archetypeIndex) => StartCoroutine(PlannerRoutine(archetypeIndex));

	private void OnDestroy()
	{
		blackboardComponent.OnGoalUpdate -= OnGoalUpdate;
	}

	private IEnumerator PlannerRoutine(int archetypeIndex)
	{
		yield return new WaitForEndOfFrame();
		
		var currentPlannerJob = new PlannerJob<TreeWatererBlackboard>(ref blackboardComponent.blackboard.blackboard, archetypeIndex, maxPlanLength, maxFScore, _actions);
		var jobHandle = currentPlannerJob.Schedule();
		yield return null;
		
		jobHandle.Complete();
		UpdatePlan(currentPlannerJob.Plan);
		currentPlannerJob.Plan.Dispose();
	}

	private void UpdatePlan(NativeArray<int> plan)
	{
		_planStack.Consume(plan, _actions);
	}
}