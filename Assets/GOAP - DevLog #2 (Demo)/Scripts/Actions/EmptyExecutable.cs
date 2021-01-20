using UnityEngine;

public class EmptyExecutable : IExecutable
{
    private EmptyExecutable()
    {
    }
    public static EmptyExecutable Instance { get; } = new EmptyExecutable();
    public IBlackboardQuery[] Preconditions { get; } = null;
    public IBlackboardModification[] Effects { get; } = null;
    public void BuildPrePlanCache()
    {
    }

    public float Cost { get; } = 0;
    public string Name { get; } = "No action.";
    public int ID { get; } = 0;
    public bool IsCompleted { get; } = false;
    
    public void OnStart()
    {
    }

    public void OnUpdate()
    {
    }

    public void OnFinish()
    {
    }

    public void OnCancel()
    {
    }

    public IExecutable GetDuplicate(GameObject target) => Instance;
}