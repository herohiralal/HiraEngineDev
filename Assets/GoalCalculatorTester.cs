using HiraEngine.Components.AI.LGOAP;
using HiraEngine.Components.AI.LGOAP.Internal;
using Unity.Collections;

namespace UnityEngine.Internal
{
	public class GoalCalculatorTester : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardComponent blackboard = null;
		[SerializeField] private GoalOrientedActionPlannerDomain domain = null;
		[HiraButton(nameof(UpdateGoal))]
		[SerializeField] private Stub updateGoal = default;
		[SerializeField] private Goal goal = null;

		private FlipFlopPool<PlannerResult> _result;

		private void Awake()
		{
			_result.First = new PlannerResult(1, Allocator.Persistent);
			_result.Second = new PlannerResult(1, Allocator.Persistent);
		}

		private void OnDestroy()
		{
			_result.First.Dispose();
			_result.Second.Dispose();
			_result = default;
		}

		public void UpdateGoal()
		{
			var jobHandle = domain.ScheduleGoalCalculatorJob(blackboard, _result.First[0], _result.Second);
			_result.Flip();
			jobHandle.Complete();
			goal = domain.Collection1[_result.First[0]];
		}
	}
}