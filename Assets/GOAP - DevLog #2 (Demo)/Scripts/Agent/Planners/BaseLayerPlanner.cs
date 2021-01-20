using UnityEngine;

[RequireComponent(typeof(TopLayerPlanner))]
public class BaseLayerPlanner : MainPlanner
{
    [SerializeField] private AbstractPlanner parent = null;
    protected override ITransitionProvider<IBlackboardQuery[]> Parent => parent;
}