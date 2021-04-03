using HiraEngine.Components.AI.LGOAP;
using UnityEngine.AI;

namespace UnityEngine.Internal
{
	public class MultiLayeredAgentComponentContainer : HiraComponentContainer, IContainsComponent<NavMeshAgent>, IContainsComponent<IBlackboardComponent>, IContainsComponent<LayeredGoalOrientedActionPlanner>
	{
		[SerializeField] private NavMeshAgent navMeshAgent;
		[SerializeField] private HiraBlackboard blackboard;
		[SerializeField] private LayeredGoalOrientedActionPlanner planner;

		NavMeshAgent IContainsComponent<NavMeshAgent>.Component => navMeshAgent;

		IBlackboardComponent IContainsComponent<IBlackboardComponent>.Component => blackboard;

		LayeredGoalOrientedActionPlanner IContainsComponent<LayeredGoalOrientedActionPlanner>.Component => planner;
	}
}