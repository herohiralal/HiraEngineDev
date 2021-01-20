using UnityEngine;

public class GoalOrientedActionPlanner : MainPlanner
{
    [SerializeField] private MainGoalProvider parent = null;
    protected override ITransitionProvider<IBlackboardQuery[]> Parent => parent;
    protected override void OnValidate()
    {
        base.OnValidate();
        if (parent == null) parent = GetComponent<MainGoalProvider>();
    }
}