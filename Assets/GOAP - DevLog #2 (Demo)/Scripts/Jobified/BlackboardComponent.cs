using System;
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
        OnBlackboardValueUpdate();
        blackboard.OnValueUpdate += OnBlackboardValueUpdate;
    }

    private void OnDestroy()
    {
        blackboard.OnValueUpdate -= OnBlackboardValueUpdate;
        _currentGoal = -1;
    }

    private void OnBlackboardValueUpdate()
    {
        var newGoal = blackboard.GetGoal(_goals).ArchetypeIndex;
        if (_currentGoal != newGoal)
        {
            OnGoalUpdate.Invoke(newGoal);
            Debug.Log($"Goal updated to: {newGoal}.");

            _currentGoal = newGoal;
        }
    }
}