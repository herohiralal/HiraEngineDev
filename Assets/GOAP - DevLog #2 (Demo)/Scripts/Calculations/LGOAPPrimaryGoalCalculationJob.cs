using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace LGOAPDemo
{
    [BurstCompile]
    public unsafe struct LGOAPPrimaryGoalCalculationJob<T> : IJob where T : unmanaged
    {
        public LGOAPPrimaryGoalCalculationJob
        (
            T* blackboard,
            NativeArray<LGOAPGoalCalculationData> goalsCalculationData,
            byte currentGoal,
            NativeArray<byte> result
        )
        {
            _blackboard = *blackboard;
            _goalsCalculationData = goalsCalculationData;
            _currentGoal = currentGoal;
            Result = result;
        }

        [ReadOnly] private T _blackboard;
        [ReadOnly] private readonly NativeArray<LGOAPGoalCalculationData> _goalsCalculationData;
        [ReadOnly] private readonly byte _currentGoal;
        [WriteOnly] public NativeArray<byte> Result;

        public void Execute()
        {
            fixed (T* blackboard = &_blackboard)
            {
                var length = _goalsCalculationData.Length;
                var data = (LGOAPGoalCalculationData*) _goalsCalculationData.GetUnsafeReadOnlyPtr();

                var goal = byte.MaxValue;
                var cachedInsistence = -1f;

                for (byte i = 0; i < length; i++)
                {
                    data += i;

                    var currentInsistence = data->GetInsistence.Invoke(blackboard);
                    var foundBetterGoal = data->GetApplicability.Invoke(blackboard) != 0 && currentInsistence > cachedInsistence;

                    goal = foundBetterGoal ? i : goal;
                    cachedInsistence = foundBetterGoal ? currentInsistence : cachedInsistence;
                }

                if (goal == byte.MaxValue)
                {
                    Result[0] = (byte) LGOAPPlannerResult.Failure;
                }
                else if (_currentGoal == goal)
                {
                    Result[0] = (byte) LGOAPPlannerResult.Unchanged;
                    Result[0] = goal;
                }
                else
                {
                    Result[0] = (byte) LGOAPPlannerResult.Success;
                    Result[1] = goal;
                }
            }
        }
    }
}