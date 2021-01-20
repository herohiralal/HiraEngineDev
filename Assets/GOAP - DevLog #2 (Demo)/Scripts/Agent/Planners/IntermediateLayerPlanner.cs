using UnityEngine;

[RequireComponent(typeof(TopLayerPlanner))]
public class IntermediateLayerPlanner : AbstractPlanner
{
    [SerializeField] private AbstractPlanner parent = null;
    protected override ITransitionProvider<IBlackboardQuery[]> Parent => parent;
}