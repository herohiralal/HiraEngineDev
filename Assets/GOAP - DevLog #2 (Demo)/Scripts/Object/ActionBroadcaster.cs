using UnityEngine;

public class ActionBroadcaster : MonoBehaviour
{
#if UNITY_EDITOR
#pragma warning disable CS0414
// ReSharper disable once NotAccessedField.Local
    [SerializeField] private HiraBlackboardKeySet keySet = null;
#pragma warning restore CS0414
#endif
    [SerializeField] private string actionName = null;

    [Space] [Header("Location Properties")] [SerializeField]
    private Transform actionLocation = null;

    [SerializeField] private float waitDuration = 1;
    [SerializeField] private float tolerance = 0.1f;

    [Space] [Header("Main Action Properties")] [SerializeField]
    private float cost = 1;

    [SerializeField] private SerializableBlackboardQuery[] preconditions = null;
    [SerializeField] private SerializableBlackboardModification[] effects = null;

    [SerializeField] private ReportOnActionExecution reportOnExecution = null;

    private IExecutable _smartObjectAction = null;

    public void OnBeginPlay()
    {
        var preconditionsLength = preconditions.Length;
        var preconditionsArray = new IBlackboardQuery[preconditionsLength];
        for (var i = 0; i < preconditionsLength; i++) preconditionsArray[i] = preconditions[i].Query;

        var effectsLength = effects.Length;
        var effectsArray = new IBlackboardModification[effectsLength];
        for (var i = 0; i < effectsLength; i++) effectsArray[i] = effects[i].Modification;

        _smartObjectAction = new SmartObjectAction(preconditionsArray,
            effectsArray,
            actionName,
            actionLocation.position,
            tolerance,
            waitDuration,
            reportOnExecution,
            cost,
            GetInstanceID());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent"))
        {
            var planner = other.GetComponent<MainPlanner>();
            if (planner == null) return;
            planner.AddAction(_smartObjectAction.GetDuplicate(other.gameObject));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Agent"))
        {
            var planner = other.GetComponent<MainPlanner>();
            if (planner != null) planner.RemoveAction(_smartObjectAction);
        }
    }
}