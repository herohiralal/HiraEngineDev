using UnityEngine;

namespace LGOAPDemo
{
    public class LGOAPAgentDomain : HiraCollection<LGOAPGoal>
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static readonly string collection1_name = "Goals";
        private static readonly string collection2_name = "Intermediate Actions";
        private static readonly string collection3_name = "Actions";
#pragma warning restore CS0414
#endif
    }
}