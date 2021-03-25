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
        [SerializeField] public Stub calculate = default;

        [HiraButton(nameof(CalculateDebug))]
        [SerializeField] public Stub calculateDebug = default;

        [SerializeField] private Action[] plan = null;

        [NonSerialized] private PlannerResult _result;
        [NonSerialized] private RawBlackboardArrayWrapper _plannerDatasets;

        private static GoalOrientedActionPlannerDomain _staticDomain = null;
        private static RawBlackboardArrayWrapper _wrapper = default;

        private void Awake()
        {
            _staticDomain = domain;
            _result = new PlannerResult(maxPlanLength, Allocator.Persistent);
            _wrapper = _plannerDatasets = new RawBlackboardArrayWrapper((byte) (maxPlanLength + 1), blackboard.Template);
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
                goalCalculator.Result.First,
                1000, 
                _plannerDatasets, blackboard,
                _result
            );

            job.Schedule().Complete();
            switch (_result.ResultType)
            {
                case PlannerResultType.Success:
                {
                    var count = _result.Count;
                    plan = new Action[count];
                    for (byte i = 0; i < count; i++)
                    {
                        plan[i] = domain.Collection2[_result[i]];
                    }

                    break;
                }
                case PlannerResultType.Failure:
                {
                    plan = new Action[0];
                    break;
                }
                case PlannerResultType.Unchanged:
                {
                    break;
                }
                case PlannerResultType.Uninitialized:
                {
                    throw new Exception("A planner provided uninitialized output.");
                }
            }
        }

        public void CalculateDebug()
        {
            goalCalculator.UpdateGoalDebug();
            var job = new MainPlannerJob(
                domain.DomainData[0],
                goalCalculator.Result.First,
                1000, 
                _plannerDatasets, blackboard,
                _result
            );
            job.Run();
            switch (_result.ResultType)
            {
                case PlannerResultType.Success:
                {
                    var count = _result.Count;
                    plan = new Action[count];
                    for (byte i = 0; i < count; i++)
                    {
                        plan[i] = domain.Collection2[_result[i]];
                    }

                    break;
                }
                case PlannerResultType.Failure:
                {
                    plan = new Action[0];
                    break;
                }
                case PlannerResultType.Unchanged:
                {
                    break;
                }
                case PlannerResultType.Uninitialized:
                {
                    throw new Exception("A planner provided uninitialized output.");
                }
            }
        }
    }
}