using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable

namespace LGOAPDemo
{
    [BurstCompile]
    public unsafe struct LGOAPMainPlannerJob<T> : IJob where T : unmanaged
    {
        public LGOAPMainPlannerJob
        (
            NativeArray<LGOAPGoalTargetData> goals,
            LGOAPGoalTargetData goal,
            NativeArray<LGOAPTaskData> actions,
            LGOAPPlannerResult parentResult,
            LGOAPPlannerResult previousResult,
            float maxFScore,
            NativeArray<T> datasets,
            LGOAPPlannerResult output
        )
        {
            _goals = goals;
            _goal = goal;
            _actions = actions;
            _parentResult = parentResult;
            _previousResult = previousResult;
            _maxFScore = maxFScore;
            _datasets = (T*) datasets.GetUnsafePtr();
            _datasetsLength = (byte) datasets.Length;
            _output = output;
        }

        // domain data
        [ReadOnly] private readonly NativeArray<LGOAPGoalTargetData> _goals;
        private LGOAPGoalTargetData _goal;
        [ReadOnly] private NativeArray<LGOAPTaskData> _actions;

        // layering
        [ReadOnly] private readonly LGOAPPlannerResult _parentResult;

        // previous run
        [ReadOnly] private readonly LGOAPPlannerResult _previousResult;

        // settings
        [ReadOnly] private readonly float _maxFScore;

        // runtime data
        [NativeDisableUnsafePtrRestriction] private T* _datasets;
        private readonly byte _datasetsLength;

        // output
        [WriteOnly] private LGOAPPlannerResult _output;

        public void Execute()
        {
            // if parent planner is uninitialized then mark self as failed
            if (_parentResult.ResultType == LGOAPPlannerResultType.Uninitialized)
            {
                Debug.LogError("Planner was run with the parent result still being uninitialized.");
                _output.ResultType = LGOAPPlannerResultType.Failure;
                _output.Count = 0;
                return;
            }

            // if parent planner failed, no point in calculating anything
            if (_parentResult.ResultType == LGOAPPlannerResultType.Failure)
            {
                _output.ResultType = LGOAPPlannerResultType.Failure;
                _output.Count = 0;
                return;
            }

            _goal = _goals[_parentResult[0]];

            // try out original plan if parent is unchanged
            if (_parentResult.ResultType == LGOAPPlannerResultType.Unchanged)
            {
                var dataset = _datasets[1];
                var count = _previousResult.Count;
                for (byte i = 0; i < count; i++)
                    _actions[_previousResult[i]].ApplyTo.Invoke(&dataset);

                // if the original plan still works, use that
                if (_goal.GetHeuristic.Invoke(&dataset) == 0)
                {
                    _previousResult.CopyTo(_output);
                    _output.ResultType = LGOAPPlannerResultType.Unchanged;
                    return;
                }
            }

            // carry on normally if not a special condition
            float threshold = _goal.GetHeuristic.Invoke(_datasets);
            float score;
            while ((score = PerformHeuristicEstimatedSearch(1, 0, threshold)) > 0 && score < _maxFScore)
                threshold = score;
            _datasets = null;
        }

        private float PerformHeuristicEstimatedSearch(byte index, float cost, float threshold)
        {
            var heuristic = _goal.GetHeuristic.Invoke(_datasets + index - 1);

            var fScore = cost + heuristic;
            if (fScore > threshold) return fScore;

            if (heuristic == 0)
            {
                _output.ResultType = LGOAPPlannerResultType.Success;
                _output.Count = (byte) (index - 1);
            }

            if (index == _datasetsLength) return float.MaxValue;

            var min = float.MaxValue;

            var count = _actions.Length;
            for (byte i = 0; i < count; i++)
            {
                var action = _actions[i];

                if (action.GetApplicability.Invoke(_datasets + index - 1) == 0) continue;
                var currentCost = cost + action.GetCost.Invoke(_datasets + index - 1);

                *(_datasets + index) = *(_datasets + index - 1);
                action.ApplyTo.Invoke(_datasets + index);

                float score;
                if ((score = PerformHeuristicEstimatedSearch((byte) (index + 1), currentCost, threshold)) < 0)
                {
                    _output[(byte) (index - 1)] = i;
                    return -1;
                }

                min = math.min(score, min);
            }

            return min;
        }
    }
}