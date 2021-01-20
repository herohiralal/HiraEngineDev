using UnityEngine;

[RequireComponent(typeof(MainPlanner))]
public class PlanRunner : MonoBehaviour
{
    [SerializeField] private MainPlanner planner = null;

    private IExecutable _currentExecutable = EmptyExecutable.Instance;

    private void OnValidate()
    {
        if (planner is null) planner = GetComponent<MainPlanner>();
    }

    private void OnEnable()
    {
        planner.OnCurrentTargetInvalidated += ExecuteNone;
        planner.OnCurrentTargetUpdate += OnActionChange;
    }

    private void OnDisable()
    {
        planner.OnCurrentTargetUpdate -= OnActionChange;
        planner.OnCurrentTargetInvalidated -= ExecuteNone;
    }

    private void Update()
    {
        if (_currentExecutable.IsCompleted)
        {
            var current = _currentExecutable;
            _currentExecutable = EmptyExecutable.Instance;
            current.OnFinish();
            planner.OnTransitionComplete();
        }
        else _currentExecutable.OnUpdate();
    }

    private void ExecuteNone()
    {
        _currentExecutable.OnCancel();
        _currentExecutable = EmptyExecutable.Instance;
    }

    private void OnActionChange(IExecutable executable)
    {
        executable.OnStart();
        _currentExecutable = executable;
    }

#if UNITY_EDITOR
#if DEBUG
    [SerializeField] private bool drawGUI = true;
    private void OnGUI()
    {
        if (drawGUI && planner != null)
        {
            GUILayout.BeginHorizontal();
            planner.DoGUI();
            GUILayout.EndHorizontal();
        }
    }
#endif
#endif
}