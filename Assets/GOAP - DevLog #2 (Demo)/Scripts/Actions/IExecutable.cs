using UnityEngine;

public interface IExecutable : IAction
{
    int ID { get; }
    bool IsCompleted { get; }
    void OnStart();
    void OnUpdate();
    void OnFinish();
    void OnCancel();
    IExecutable GetDuplicate(GameObject target);
}