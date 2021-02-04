using UnityEngine;

namespace Graphview.Scripts
{
	public class Response : ScriptableObject
	{
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
#endif
		
		[SerializeField] [HiraCollectionDropdown(typeof(Dialogue))] private Dialogue next = null;
		public Dialogue Next => next;
	}
}