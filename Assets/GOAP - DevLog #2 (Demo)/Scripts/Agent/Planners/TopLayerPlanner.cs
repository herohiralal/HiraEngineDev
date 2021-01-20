using UnityEngine;

public class TopLayerPlanner : AbstractPlanner
{
    [SerializeField] private MainGoalProvider parent = null;
    protected override ITransitionProvider<IBlackboardQuery[]> Parent => parent;
    protected override void OnValidate()
    {
        base.OnValidate();
        if (parent == null) parent = GetComponent<MainGoalProvider>();
    }
}