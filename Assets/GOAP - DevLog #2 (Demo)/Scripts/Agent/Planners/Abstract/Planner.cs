using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HiraEngine.Components.Planner;
using UnityEngine;

[RequireComponent(typeof(MainGoalProvider))]
public abstract class Planner<TActionType, TTransitionProviderType> : MonoBehaviour,
    ITransitionProvider<TTransitionProviderType> where TActionType : IAction
{
    [SerializeField] private byte maxPlanLength = 15;
    [SerializeField] private HiraBlackboard blackboard = null;
    protected abstract ITransitionProvider<IBlackboardQuery[]> Parent { get; }

    // ReSharper disable once Unity.RedundantSerializeFieldAttribute
    [SerializeField] protected List<TActionType> actions = null;
    private IPlanner<TActionType> _planner = null;
    private IPlanStack<TActionType> _plan = null;
    private bool _planSchedulerRunning = false;

    private IBlackboardQuery[] _currentGoal = null;

    public event Action<TTransitionProviderType> OnCurrentTargetUpdate = delegate { };
    public event Action OnCurrentTargetInvalidated = delegate { };

    protected void OnEnable()
    {
        actions = new List<TActionType>();
        _planner = PlannerTypes.GetPlanner<TActionType>(blackboard, maxPlanLength);
        _plan = PlannerTypes.GetPlanStack<TActionType>(maxPlanLength);
        Parent.OnCurrentTargetUpdate += OnParentTargetUpdate;
        Parent.OnCurrentTargetInvalidated += OnParentTargetInvalidation;
    }

    protected void OnDisable()
    {
        Parent.OnCurrentTargetInvalidated -= OnParentTargetInvalidation;
        Parent.OnCurrentTargetUpdate -= OnParentTargetUpdate;
        StopPlannerIfRunning();
        actions = null;
        _planner = null;
        _plan = null;
    }

    private void OnParentTargetUpdate(IBlackboardQuery[] newGoal)
    {
        _currentGoal = newGoal;
        StartPlanner();
    }

    private void OnParentTargetInvalidation()
    {
        _currentGoal = null;
        _plan.Invalidate();
        StopPlannerIfRunning();
        OnCurrentTargetInvalidated.Invoke();
    }

    public void OnTransitionComplete()
    {
        if (_planSchedulerRunning || _planner.IsActive)
        {
            HiraLogger.LogError("Planner / plan scheduler" +
                                " running when goal was reported completed.", this);
            return;
        }

        if (_plan.HasActions) OnCurrentTargetUpdate.Invoke(GetTargetFromAction(_plan.Pop()));
        else Parent.OnTransitionComplete();
    }

    protected abstract TTransitionProviderType GetTargetFromAction(TActionType action);

    public void DoGUI()
    {
        Parent.DoGUI();
        GUILayout.BeginVertical();
        GUILayout.Label(_plan.ToString());
        GUILayout.EndVertical();
    }

    public virtual void AddAction(TActionType action)
    {
    }

    public virtual void RemoveAction(TActionType action)
    {
    }

    protected virtual void OnValidate()
    {
        if (blackboard == null) blackboard = GetComponent<HiraBlackboard>();
    }

    #region Planning

    private void StartPlanner()
    {
        if (_planSchedulerRunning) return;

        if (_planner.IsActive) RunPlanScheduler();
        else RunPlanner();
    }

    private void StopPlannerIfRunning()
    {
        if (_planSchedulerRunning || _planner.IsActive) _plannerCancellationTokenSource.Cancel();
    }

    private CancellationTokenSource _plannerCancellationTokenSource = new CancellationTokenSource();

    private async void RunPlanScheduler()
    {
        _planSchedulerRunning = true;
        _plannerCancellationTokenSource.Cancel();
        _plannerCancellationTokenSource = new CancellationTokenSource();
        while (!_plannerCancellationTokenSource.IsCancellationRequested && _planner.IsActive) await Task.Yield();
        _planSchedulerRunning = false;

        if (!_plannerCancellationTokenSource.IsCancellationRequested) RunPlanner();
        else _plannerCancellationTokenSource = new CancellationTokenSource();
    }

    private void RunPlanner()
    {
        _planner.Initialize()
            .ForGoal(_currentGoal)
            .WithAvailableTransitions(actions.ToArray())
            .WithCancellationToken(_plannerCancellationTokenSource.Token)
            .WithMaxFScore(100)
            .WithCallback(PlannerCallback)
            .RunMultiThreaded();
        Debug.Log("Running planner.");
    }

    private void PlannerCallback(PlannerResult result, TActionType[] plan, int planLength)
    {
        switch (result)
        {
            case PlannerResult.None:
                HiraLogger.LogError("<color=red>Unhandled planner output.</color>.", this);
                break;
            case PlannerResult.Success:
                _plan.Consume(plan, planLength);
                OnCurrentTargetUpdate.Invoke(GetTargetFromAction(_plan.Pop()));
                break;
            case PlannerResult.Failure:
                HiraLogger.LogError("<color=red>Planner failed!</color>.", this);
                break;
            case PlannerResult.Cancelled:
                _plannerCancellationTokenSource = new CancellationTokenSource();
                HiraLogger.Log("Planner cancelled.", this);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }

    #endregion
}