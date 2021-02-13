using System;
using UnityEngine;

public class ActionRunner : MonoBehaviour
{
    [SerializeField] private AIComponent brain = null;
    private IActualAction _currentAction = EmptyAction.INSTANCE;

    private void Awake()
    {
        brain.OnCurrentTargetUpdate += OnTargetUpdate;
    }

    private void OnDestroy()
    {
        brain.OnCurrentTargetUpdate -= OnTargetUpdate;
    }

    private void OnTargetUpdate(IActualAction action)
    {
        action.Abort();
        action.Initialize(gameObject);
        _currentAction = action;
    }

    private void Update()
    {
        switch (_currentAction.Update())
        {
            case ActionStatus.Running:
                break;
            case ActionStatus.Successful:
                _currentAction = EmptyAction.INSTANCE;
                brain.OnTransitionComplete(true);
                break;
            case ActionStatus.Failed:
                _currentAction = EmptyAction.INSTANCE;
                brain.OnTransitionComplete(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}