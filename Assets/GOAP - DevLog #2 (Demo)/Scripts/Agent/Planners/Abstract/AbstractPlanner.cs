using UnityEngine;

public abstract class AbstractPlanner : Planner<SerializablePlannerHybridTransition, IBlackboardQuery[]>
{
    protected override IBlackboardQuery[] GetTargetFromAction(SerializablePlannerHybridTransition action) => 
        action.Targets;
}