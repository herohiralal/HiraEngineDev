using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace LGOAPDemo
{
    public readonly struct LGOAPGoalCalculationData
    {
        public LGOAPGoalCalculationData
        (
            FunctionPointer<LGOAPTransitionApplicabilityCheckDelegate> getApplicability,
            FunctionPointer<LGOAPGoalInsistenceCheckDelegate> getInsistence
        )
        {
            GetApplicability = getApplicability;
            GetInsistence = getInsistence;
        }

        [ReadOnly] public readonly FunctionPointer<LGOAPTransitionApplicabilityCheckDelegate> GetApplicability;
        [ReadOnly] public readonly FunctionPointer<LGOAPGoalInsistenceCheckDelegate> GetInsistence;
    }

    public readonly struct LGOAPGoalTargetData
    {
        public LGOAPGoalTargetData(FunctionPointer<LGOAPGoalHeuristicCheckDelegate> getHeuristic)
        {
            GetHeuristic = getHeuristic;
        }

        [ReadOnly] public readonly FunctionPointer<LGOAPGoalHeuristicCheckDelegate> GetHeuristic;
    }

    public class LGOAPGoal : ScriptableObject, IHiraCollectionAwareTarget
    {
        public HiraCollection ParentCollection { get; set; }
        public int Index { get; set; }
    }
}