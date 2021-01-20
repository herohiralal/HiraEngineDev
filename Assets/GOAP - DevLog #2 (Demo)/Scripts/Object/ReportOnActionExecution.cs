using System;
using UnityEngine;
using UnityEngine.Events;

public class ReportOnActionExecution : MonoBehaviour
{
    [SerializeField] private float delay = 0;
    [SerializeField] private UnityGameObjectEvent onReport = null;

    public void Report(GameObject target)
    {
        if (delay == 0f) onReport.Invoke(target);
        else HiraTimerEvents.RequestPing(() => onReport.Invoke(target), delay);
    }
}

[Serializable]
public class UnityGameObjectEvent : UnityEvent<GameObject>
{
}