using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(HiraBlackboard))]
public class MainGoalProvider : MonoBehaviour, ITransitionProvider<IBlackboardQuery[]>
{
    [SerializeField] private HiraBlackboard blackboard = null;
    [SerializeField] private SerializablePlannerTransition[] goals = null;
    [SerializeField] private SerializablePlannerTransition fallbackGoal = null;

    private SerializablePlannerTransition _currentGoal = null;
    
    public event Action<IBlackboardQuery[]> OnCurrentTargetUpdate = delegate { };
    public event Action OnCurrentTargetInvalidated = delegate { };

    private void OnEnable()
    {
        blackboard.Initialize<object>(null);
        blackboard.OnValueUpdate += CheckGoalChange;
    }

    private void OnDisable()
    {
        blackboard.OnValueUpdate -= CheckGoalChange;
        _currentGoal = null;
        blackboard.OnDispossess();
    }

    public void CheckGoalChange()
    {
        var newGoal = CalculateGoal();
        newGoal = newGoal == null ? fallbackGoal : newGoal;

        if (newGoal != _currentGoal)
        {
            _currentGoal = newGoal;
            OnCurrentTargetUpdate.Invoke(_currentGoal.Targets);
        }
    }

    private SerializablePlannerTransition CalculateGoal() =>
        goals.FirstOrDefault(t =>
            t.Preconditions.IsSatisfiedBy(blackboard.DataSet)
            && t.Targets.IsNotSatisfiedBy(blackboard.DataSet));

    public void OnTransitionComplete()
    {
        if (_currentGoal == fallbackGoal) OnCurrentTargetUpdate.Invoke(_currentGoal.Targets);
    }

    private void OnValidate()
    {
        if (blackboard == null) blackboard = GetComponent<HiraBlackboard>();
    }

    public void DoGUI()
    {
        GUILayout.BeginVertical();
        blackboard.DoGUI();
        if (_currentGoal != null) GUILayout.Label($"Current Goal: {_currentGoal.name}.");
        GUILayout.EndVertical();
    }
}