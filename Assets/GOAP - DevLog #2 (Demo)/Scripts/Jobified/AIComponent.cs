using System;
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
	public event Action<IActualAction> OnCurrentTargetUpdate = delegate { };
	private IActualAction _currentAction = null;

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

	public void AddAction(IActualAction action)
	{
		if (!_actions.Contains(action)) _actions.Add(action);
	}

	public void RemoveAction(IActualAction action)
	{
		var indexToRemove = _actions.FindIndex(a=>a==action);
		if (indexToRemove > 0) _actions.RemoveAt(indexToRemove);
	}

	private IEnumerator PlannerRoutine(int archetypeIndex)
	{
		yield return new WaitForEndOfFrame();
		
		var currentPlannerJob = new PlannerJob<TreeWatererBlackboard>(ref blackboardComponent.blackboard.blackboard, archetypeIndex, maxPlanLength, maxFScore, _actions);
		var jobHandle = currentPlannerJob.Schedule();
		yield return null;
		
		jobHandle.Complete();
		_planStack.Consume(currentPlannerJob.Plan, _actions);
		OnCurrentTargetUpdate.Invoke(_currentAction = _planStack.Pop());
		currentPlannerJob.Plan.Dispose();
	}

	public void OnTransitionComplete(bool success)
	{
		if (success)
		{
			var previousAction = _currentAction;

			var planStackHasActions = _planStack.HasActions;
			if (planStackHasActions) OnCurrentTargetUpdate.Invoke(_currentAction = _planStack.Pop());

			blackboardComponent.OnActionSuccess(previousAction.Data.ArchetypeIndex, !planStackHasActions);
		}
		else blackboardComponent.RecalculateGoal();
	}
}