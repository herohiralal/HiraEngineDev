using UnityEngine;

namespace LGOAPDemo
{
	public class LGOAPAgentDomain3Layers : HiraCollection<LGOAPGoal, LGOAPTask, LGOAPIntermediateGoal, LGOAPIntermediateGoal>, ILGOAPDomain
	{
#if UNITY_EDITOR
#pragma warning disable CS0414
		private static readonly string collection1_name = "Goals";
		private static readonly string collection2_name = "Tasks";
		private static readonly string collection3_name = "Intermediate Goals";
		private static readonly string collection4_name = "Intermediate Goals";
#pragma warning restore CS0414
#endif
	}
}