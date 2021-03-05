using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace LGOAPDemo
{
    public readonly struct LGOAPTaskData
    {
        public LGOAPTaskData
        (
            FunctionPointer<LGOAPCostCheckDelegate> getCost,
            FunctionPointer<LGOAPTransitionApplicabilityCheckDelegate> getApplicability,
            FunctionPointer<LGOAPModificationDelegate> applyTo
        )
        {
            GetCost = getCost;
            GetApplicability = getApplicability;
            ApplyTo = applyTo;
        }

        [ReadOnly] public readonly FunctionPointer<LGOAPCostCheckDelegate> GetCost;
        [ReadOnly] public readonly FunctionPointer<LGOAPTransitionApplicabilityCheckDelegate> GetApplicability;
        [ReadOnly] public readonly FunctionPointer<LGOAPModificationDelegate> ApplyTo;
    }

	public class LGOAPTask : ScriptableObject, IHiraCollectionAwareTarget
	{
        public HiraCollection ParentCollection { get; set; }
        public int Index { get; set; }
    }
}