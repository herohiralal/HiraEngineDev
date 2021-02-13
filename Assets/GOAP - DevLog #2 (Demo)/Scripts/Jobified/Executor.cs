using System.Threading;

namespace UnityEngine
{
	public class Executor : MonoBehaviour
	{
		[SerializeField] private GameObject agent = null;
		[SerializeField] private Transform agentLocation = null;

		private void Awake()
		{
			ThreadPool.QueueUserWorkItem(DoNothing);
			ThreadPool.QueueUserWorkItem(DoNothing);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				var instantiatedAgent = Instantiate(agent, agentLocation.position, agentLocation.rotation, null);

				var blackboard = instantiatedAgent.GetComponent<BlackboardComponent>();

				HiraTimerEvents.RequestPing(blackboard.RecalculateGoal, 1f);
				Destroy(gameObject);
			}
		}

		private static void DoNothing(object state)
		{
		}
	}
}