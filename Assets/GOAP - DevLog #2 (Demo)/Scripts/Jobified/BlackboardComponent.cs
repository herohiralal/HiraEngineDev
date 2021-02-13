using System;
using UnityEngine;

public class BlackboardComponent : MonoBehaviour
{
    [SerializeField] public TreeWatererBlackboardWrapper blackboard = null;
    public event Action<int> OnGoalUpdate = delegate {  };

    private readonly GoalData[] _goals =
    {
        new GoalData(TreeWatererBlackboard.ArchetypeIndices.GOAL_WATER_PLANT, 10),
        new GoalData(TreeWatererBlackboard.ArchetypeIndices.GOAL_PATROL, 1),
    };

    private int _currentGoal = -1;

    private bool _updateGoalOnBlackboardValueUpdate;

    private void Awake()
    {
        _updateGoalOnBlackboardValueUpdate = true;
        blackboard = new TreeWatererBlackboardWrapper();
        
        blackboard.OnValueUpdate += OnBlackboardValueUpdate;
    }

    private void OnDestroy()
    {
        blackboard.OnValueUpdate -= OnBlackboardValueUpdate;
        _currentGoal = -1;
    }

    public void RecalculateGoal() => CalculateGoal(true);
    private void OnBlackboardValueUpdate()
    {
        if (_updateGoalOnBlackboardValueUpdate) CalculateGoal();
    }

    private void CalculateGoal(bool forceBroadcastGoalUpdateEvent = false)
    {
        var newGoal = blackboard.GetGoal(_goals).ArchetypeIndex;
        if (forceBroadcastGoalUpdateEvent || _currentGoal != newGoal)
        {
            OnGoalUpdate.Invoke(newGoal);
            _currentGoal = newGoal;
        }
    }

    public void OnActionSuccess(int archetypeIndex, bool final)
    {
        _updateGoalOnBlackboardValueUpdate = false;
        {
            blackboard.ApplyActionEffect(archetypeIndex);
            if (final)
            {
                blackboard.Insecurity = 100;
                _currentGoal = -1;
            }
        }
        _updateGoalOnBlackboardValueUpdate = true;

        CalculateGoal();
    }
}