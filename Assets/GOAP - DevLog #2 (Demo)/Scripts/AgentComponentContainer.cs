using HiraEngine.Components.AI.LGOAP;
using UnityEngine.AI;

namespace UnityEngine.Internal
{
	public class AgentComponentContainer : HiraComponentContainer, IContainsComponent<NavMeshAgent>, IContainsComponent<IBlackboardComponent>, IContainsComponent<GoalOrientedActionPlanner>
	{
		[SerializeField] private NavMeshAgent navMeshAgent;
		[SerializeField] private HiraBlackboard blackboard;
		[SerializeField] private GoalOrientedActionPlanner planner;

		NavMeshAgent IContainsComponent<NavMeshAgent>.Component => navMeshAgent;

		IBlackboardComponent IContainsComponent<IBlackboardComponent>.Component => blackboard;

		GoalOrientedActionPlanner IContainsComponent<GoalOrientedActionPlanner>.Component => planner;
	}
}