using UnityEngine;
using UnityEngine.AI;

public class SmartObjectAction : IExecutable
{
    public SmartObjectAction(IBlackboardQuery[] preconditions,
        IBlackboardModification[] effects,
        string name,
        Vector3 targetPosition,
        float tolerance,
        float waitDuration,
        ReportOnActionExecution toReport,
        float cost,
        int id,
        GameObject target = null)
    {
        Preconditions = preconditions;
        Effects = effects;
        Name = name;
        _targetPosition = targetPosition;
        _tolerance = tolerance;
        _waitDuration = waitDuration;
        _toReport = toReport;
        _baseCost = cost;
        ID = id;
        _target = target;
    }

    public string Name { get; }
    public sealed override string ToString() => Name;
    public IBlackboardQuery[] Preconditions { get; }
    public IBlackboardModification[] Effects { get; }
    private readonly Vector3 _targetPosition;
    private readonly float _tolerance;
    private float _timeSinceStart;
    private readonly float _waitDuration;
    private readonly ReportOnActionExecution _toReport;
    private readonly float _baseCost;
    private readonly GameObject _target;
    private bool _reached = false;
    public int ID { get; }

    public float Cost { get; private set; }

    public void BuildPrePlanCache() =>
        Cost = Mathf.Floor(Vector3.Distance(_target.transform.position, _targetPosition))
               + _baseCost;

    public void OnStart()
    {
        _timeSinceStart = 0;
        _reached = false;

        var navMeshAgent = _target.transform.GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = _tolerance;
        navMeshAgent.destination = _targetPosition;
    }

    public bool IsCompleted => _reached && _timeSinceStart >= _waitDuration;

    public void OnUpdate()
    {
        _reached = _reached || Vector3.Distance(_target.transform.position, _targetPosition) < _tolerance;
        _timeSinceStart += _reached ? Time.deltaTime : 0;
    }

    public void OnFinish() => _toReport.Report(_target);

    public void OnCancel()
    {
    }

    public IExecutable GetDuplicate(GameObject target) =>
        new SmartObjectAction(Preconditions,
            Effects,
            Name,
            _targetPosition,
            _tolerance,
            _waitDuration,
            _toReport,
            _baseCost,
            ID,
            target);
}