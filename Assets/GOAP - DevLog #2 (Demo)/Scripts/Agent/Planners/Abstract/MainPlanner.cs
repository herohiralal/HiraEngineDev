public abstract class MainPlanner : Planner<IExecutable, IExecutable>
{

    protected override IExecutable GetTargetFromAction(IExecutable action) => 
        action;

    public override void AddAction(IExecutable action) => actions.Add(action);

    public override void RemoveAction(IExecutable action)
    {
        for (var i = actions.Count - 1; i > -1; i--)
            if (actions[i].ID == action.ID)
                actions.RemoveAt(i);
    }
}