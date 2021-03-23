using System;
using HiraEngine.Components.AI.LGOAP;
using HiraEngine.Components.AI.LGOAP.Internal;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Internal
{
	public class GoalCalculatorTester : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardComponent blackboard = null;
		[SerializeField] private GoalOrientedActionPlannerDomain domain = null;
		[HiraButton(nameof(UpdateGoal))]
		[SerializeField] private Stub updateGoal = default;
		[HiraButton(nameof(UpdateGoalDebug))]
		[SerializeField] private Stub updateGoalDebug = default;
		[SerializeField] private Goal goal = null;

		[NonSerialized] public FlipFlopPool<PlannerResult> Result;

		private void Awake()
		{
			Result.First = new PlannerResult(1, Allocator.Persistent){Count = 1, [0] = byte.MaxValue};
			Result.Second = new PlannerResult(1, Allocator.Persistent){Count = 1, [0] = byte.MaxValue};
		}

		private void OnDestroy()
		{
			Result.First.Dispose();
			Result.Second.Dispose();
			Result = default;
		}

		public unsafe void UpdateGoal()
		{
			var jobHandle = new GoalCalculatorJob(blackboard.Data, domain.InsistenceCalculatorsBlock, Result.First[0], Result.Second).Schedule();
			Result.Flip();
			jobHandle.Complete();
			goal = domain.Collection1[Result.First[0]];
		}

        public unsafe void UpdateGoalDebug()
        {
            new GoalCalculatorJob(blackboard.Data, domain.InsistenceCalculatorsBlock, Result.First[0], Result.Second).Run();
            Result.Flip();
            goal = domain.Collection1[Result.First[0]];
        }
	}
}