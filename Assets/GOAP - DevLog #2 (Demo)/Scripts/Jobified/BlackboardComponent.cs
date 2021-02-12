using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardComponent : MonoBehaviour
{
    [SerializeField] private TreeWatererBlackboardWrapper blackboard = null;
    public event Action<int> OnGoalUpdate = delegate {  };

    private readonly GoalData[] _goals =
    {
        new GoalData(TreeWatererBlackboard.ArchetypeIndices.GOAL_WATER_PLANT, 10),
        new GoalData(TreeWatererBlackboard.ArchetypeIndices.GOAL_PATROL, 1),
    };

    private int _currentGoal = -1;

    private void Awake()
    {
        blackboard = new TreeWatererBlackboardWrapper();
        blackboard.OnValueUpdate += OnBlackboardValueUpdate;
    }

    private void OnDestroy()
    {
        blackboard.OnValueUpdate -= OnBlackboardValueUpdate;
        _currentGoal = -1;
    }

    public void Initialize() => OnBlackboardValueUpdate();

    private void OnBlackboardValueUpdate()
    {
        var newGoal = blackboard.GetGoal(_goals).ArchetypeIndex;
        if (_currentGoal != newGoal)
        {
            OnGoalUpdate.Invoke(newGoal);
            _currentGoal = newGoal;
        }
    }

    public PlannerJob<TreeWatererBlackboard> PlanWith(List<IActualAction> actions)
    {
        return new PlannerJob<TreeWatererBlackboard>(ref blackboard.blackboard, _currentGoal, 15, 100, actions);
    }
}