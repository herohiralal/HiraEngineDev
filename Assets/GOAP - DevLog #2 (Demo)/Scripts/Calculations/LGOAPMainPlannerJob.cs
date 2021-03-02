using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace LGOAPDemo
{
    public unsafe struct LGOAPMainPlannerJob : IJob
    {
        // domain info
        [ReadOnly] private readonly NativeArray<LGOAPGoalTargetData> _targetData;
        [ReadOnly] private readonly NativeArray<LGOAPTaskData> _actions;
        
        // current status
        [ReadOnly] private readonly NativeArray<byte> _currentPlan;
        
        // parent data
        [ReadOnly] private readonly NativeArray<byte> _parentResult;
        private LGOAPGoalTargetData _goal;

        // result
        [WriteOnly] public NativeArray<byte> Result;

        public void Execute()
        {
            if (_parentResult[0] == (byte) LGOAPPlannerResult.Failure)
            {
                Result[0] = (byte) LGOAPPlannerResult.Failure;
            }
            else if (_parentResult[0] == (byte) LGOAPPlannerResult.Unchanged)
            {
                _goal = _targetData[1];

                var currentPlan = (byte*)_currentPlan.GetUnsafeReadOnlyPtr();
                var currentPlanSize = *currentPlan;

                var actions = _actions.GetUnsafeReadOnlyPtr();

                var actionIt = currentPlan + 1;

                for (byte i = 0; i < currentPlanSize; i++)
                {
                    
                }
            }
        }
    }
}