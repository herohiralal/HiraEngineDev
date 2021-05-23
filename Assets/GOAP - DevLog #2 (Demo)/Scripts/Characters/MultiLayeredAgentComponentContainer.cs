using HiraEngine.Components.AI.LGOAP;
using UnityEngine.AI;

namespace UnityEngine.Internal
{
	public class MultiLayeredAgentComponentContainer : HiraComponentContainer,
		IContainsComponent<NavMeshAgent>,
		IContainsComponent<IBlackboardComponent>,
		IContainsComponent<LayeredGoalOrientedActionPlanner>,
		IContainsComponent<AgentAnimator>
	{
		[SerializeField] private AgentAnimator animator;
		[SerializeField] private NavMeshAgent navMeshAgent;
		[SerializeField] private HiraBlackboard blackboard;
		[SerializeField] private LayeredGoalOrientedActionPlanner planner;

		AgentAnimator IContainsComponent<AgentAnimator>.Component => animator;

		NavMeshAgent IContainsComponent<NavMeshAgent>.Component => navMeshAgent;

		IBlackboardComponent IContainsComponent<IBlackboardComponent>.Component => blackboard;

		LayeredGoalOrientedActionPlanner IContainsComponent<LayeredGoalOrientedActionPlanner>.Component => planner;
	}
}