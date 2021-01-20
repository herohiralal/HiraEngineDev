using System.Threading;
using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] private GameObject agent = null;
    [SerializeField] private Transform agentLocation = null;
    [SerializeField] private HiraBlackboardKeySet keySet = null;
    [SerializeField] private SerializablePlannerTransition[] transitions = null;
    [SerializeField] [NonReorderable] private SerializableBlackboardModification[] startupModifications = null;

    [SerializeField] private ActionBroadcaster[] actionBroadcasters = null;
    [SerializeField] private BlackboardModifier[] modifiers = null;

    private void Awake()
    {
        keySet.Initialize();
        foreach (var transition in transitions) transition.Initialize();
        foreach (var modification in startupModifications) modification.Modification.ApplyTo(keySet.ValueAccessor);

        foreach (var broadcaster in actionBroadcasters) broadcaster.OnBeginPlay();
        foreach (var modifier in modifiers) modifier.OnBeginPlay();

        // to initialize some threads into the pool
        ThreadPool.QueueUserWorkItem(DoNothing);
        ThreadPool.QueueUserWorkItem(DoNothing);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var instantiatedAgent = Instantiate(agent, agentLocation.position, agentLocation.rotation, null);

            var goalProvider = instantiatedAgent.GetComponent<MainGoalProvider>();
            goalProvider.enabled = true;

            var mainPlanner = instantiatedAgent.GetComponent<MainPlanner>();
            if (!(mainPlanner is null)) mainPlanner.enabled = true;
            
            HiraTimerEvents.RequestPing(goalProvider.CheckGoalChange, 1f);
            Destroy(gameObject);
        }
    }

    private static void DoNothing(object obj = null)
    {
    }
}