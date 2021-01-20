using System;

public interface ITransitionProvider<out T>
{
    event Action<T> OnCurrentTargetUpdate;
    event Action OnCurrentTargetInvalidated;
    void OnTransitionComplete();
    void DoGUI();
}