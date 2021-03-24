using System;
using HiraEngine.Components.AI.LGOAP;
using HiraEngine.Components.AI.LGOAP.Internal;
using HiraEngine.Components.Blackboard.Raw;
using Unity.Collections;
using Unity.Jobs;
using Action = HiraEngine.Components.AI.LGOAP.Action;

namespace UnityEngine.Internal
{
    public class PlannerTester : MonoBehaviour
    {
        [SerializeField] private HiraBlackboardComponent blackboard = null;
        [SerializeField] private GoalOrientedActionPlannerDomain domain = null;
        [SerializeField] private GoalCalculatorTester goalCalculator = null;
        [SerializeField] private byte maxPlanLength = 10;

        [HiraButton(nameof(Calculate))]
        [SerializeField] private Stub calculate = default;

        [HiraButton(nameof(CalculateDebug))]
        [SerializeField] private Stub calculateDebug = default;

        [SerializeField] private byte[] plan = null;

        [NonSerialized] private PlannerResult _result;
        [NonSerialized] private RawBlackboardArrayWrapper _plannerDatasets;

        private static GoalOrientedActionPlannerDomain _staticDomain = null;

        private void Awake()
        {
            _staticDomain = domain;
            _result = new PlannerResult(maxPlanLength, Allocator.Persistent);
            _plannerDatasets = new RawBlackboardArrayWrapper((byte) (maxPlanLength + 1), blackboard.Template);
        }

        private void OnDestroy()
        {
            _plannerDatasets.Dispose();
            _result.Dispose();
        }

        public void Calculate()
        {
            goalCalculator.UpdateGoal();
            var job = new MainPlannerJob(
                domain.DomainData[0],
                goalCalculator.Result.First, 0,
                1000, 
                _plannerDatasets, blackboard,
                _result
            );

            job.Schedule().Complete();
            if (_result.ResultType == PlannerResultType.Success)
            {
                var count = _result.Count;
                plan = new byte[count];
                for (byte i = 0; i < count; i++)
                {
                    plan[i] = _result[i];
                }
            }
        }

        public void CalculateDebug()
        {
            goalCalculator.UpdateGoalDebug();
            var job = new MainPlannerJob(
                domain.DomainData[0],
                goalCalculator.Result.First, 0,
                1000, 
                _plannerDatasets, blackboard,
                _result
            );
            job.Run();
            if (_result.ResultType == PlannerResultType.Success)
            {
                var count = _result.Count;
                plan = new byte[count];
                for (byte i = 0; i < count; i++)
                {
                    plan[i] = _result[i];
                }
            }
        }
    }
}