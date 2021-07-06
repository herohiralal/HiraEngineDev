using UnityEngine.AI;

namespace UnityEngine.Internal
{
	public class MultiLayeredAgentComponentContainer : HiraComponentContainer,
		IContainsComponent<NavMeshAgent>,
		IContainsComponent<AgentAnimator>
	{
		[SerializeField] private AgentAnimator animator;
		[SerializeField] private NavMeshAgent navMeshAgent;

		AgentAnimator IContainsComponent<AgentAnimator>.Component => animator;

		NavMeshAgent IContainsComponent<NavMeshAgent>.Component => navMeshAgent;
	}
}