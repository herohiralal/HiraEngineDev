using UnityEngine;

namespace LGOAPDemo
{
	public class LGOAPIntermediateGoal : ScriptableObject, IHiraCollectionAwareTarget
	{
		public HiraCollection ParentCollection { get; set; }
		public int Index { get; set; }
	}
}