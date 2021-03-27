using System.Collections.Generic;
using HiraEngine.Components.AI.LGOAP;

namespace UnityEngine.Internal
{
	[System.Serializable, DynamicallyAccessibleEnum("6e8da6b9-998f-4186-81e4-7a9d11fc1ebd"), System.Flags]
	public enum Class : byte
	{
		None = 0, Warrior = 1 << 0, Mage = 1 << 1, Rogue = 1 << 2
	}

	public class Initializer : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardTemplate template = null;
		[SerializeField] private GoalOrientedActionPlannerDomain domain = null;

		[SerializeField] private GameObject agentPrefab = null;
		private readonly Stack<IInitializable> _initializedObjects = new Stack<IInitializable>();

		private void Awake()
		{
			object init = null;

			template.Initialize(ref init);
			_initializedObjects.Push(template);

			domain.Initialize(ref init);
			_initializedObjects.Push(domain);
			
			var agent = Instantiate(agentPrefab, transform.position, Quaternion.identity, null);

			var blackboard = agent.GetComponent<HiraBlackboard>();
			if (blackboard != null && blackboard is IInitializable initializableBlackboard)
			{
				initializableBlackboard.Initialize(ref init);
				_initializedObjects.Push(initializableBlackboard);
			}

			var planner = agent.GetComponent<GoalOrientedActionPlanner>();
			if (planner != null)
			{
				planner.Initialize(ref init);
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