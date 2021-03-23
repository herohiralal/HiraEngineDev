using HiraEngine.Components.AI.LGOAP;
using HiraEngine.Components.AI.LGOAP.Internal;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Internal
{
    public class PlannerTester : MonoBehaviour
    {
        [SerializeField] private HiraBlackboardComponent blackboard = null;
        [SerializeField] private GoalOrientedActionPlannerDomain domain = null;
        [SerializeField] private GoalCalculatorTester goalCalculator = null;

        [HiraButton(nameof(Calculate))]
        [SerializeField] private Stub calculate = default;

        [HiraButton(nameof(CalculateDebug))]
        [SerializeField] private Stub calculateDebug = default;

        private PlannerResult _result;
        private static GoalOrientedActionPlannerDomain _staticDomain = null;

        private void Awake()
        {
            _staticDomain = domain;
            _result = new PlannerResult(1, Allocator.Persistent);
        }

        private void OnDestroy()
        {
            _result.Dispose();
        }

        public unsafe void Calculate()
        {
            goalCalculator.UpdateGoal();
            var job = new MainPlannerJob(
                domain.GetLayer(0),
                goalCalculator.Result.First,
                0,
                ((byte*) blackboard.Data.GetUnsafeReadOnlyPtr()), 
                _result);

            job.Schedule().Complete();
            if(_result.ResultType == PlannerResultType.Success) Debug.Log($"Result: {_result[0].ToBoolean()}.");
        }

        public unsafe void CalculateDebug()
        {
            goalCalculator.UpdateGoalDebug();
            var job = new MainPlannerJob(
                domain.GetLayer(0),
                goalCalculator.Result.First,
                0,
                ((byte*) blackboard.Data.GetUnsafeReadOnlyPtr()), 
                _result);
            job.Run();
            if(_result.ResultType == PlannerResultType.Success) Debug.Log($"Result: {_result[0].ToBoolean()}.");
        }
    }
}