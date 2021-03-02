using UnityEngine;

namespace LGOAPDemo
{
	public class GOAPAgentDomain : HiraCollection<LGOAPGoal, LGOAPTask>, IAgentDomain
	{
#if UNITY_EDITOR
#pragma warning disable CS0414
		private static readonly string collection1_name = "Goals";
		private static readonly string collection2_name = "Tasks";
#pragma warning restore CS0414
#endif
	}
}