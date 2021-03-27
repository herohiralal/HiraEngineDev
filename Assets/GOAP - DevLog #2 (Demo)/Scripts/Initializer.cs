using System.Collections.Generic;
using HiraEngine.Components.AI.LGOAP;

namespace UnityEngine.Internal
{
    public class Initializer : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardTemplate template = null;
		[SerializeField] private GoalOrientedActionPlannerDomain domain = null;

		[SerializeField] private GameObject agentPrefab = null;
		private readonly Stack<IInitializable> _initializedObjects = new Stack<IInitializable>();

		private void Awake()
		{
            template.Initialize();
			_initializedObjects.Push(template);

			domain.Initialize();
			_initializedObjects.Push(domain);
			
			var agent = Instantiate(agentPrefab, transform.position, Quaternion.identity, null);

			var blackboard = agent.GetComponent<HiraBlackboard>();
			if (blackboard != null && blackboard is IInitializable initializableBlackboard)
			{
				initializableBlackboard.Initialize();
				_initializedObjects.Push(initializableBlackboard);
			}

			var planner = agent.GetComponent<GoalOrientedActionPlanner>();
			if (planner != null)
			{
				planner.Initialize();
				_initializedObjects.Push(planner);
			}
		}

		private void OnDestroy()
		{
			while (_initializedObjects.Count > 0)
			{
				_initializedObjects.Pop().Shutdown();
			}
		}
	}
}