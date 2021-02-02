using UnityEngine;

namespace Graphview.Scripts
{
	public class DialogueResponse : ScriptableObject
	{
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
#endif
		
		[SerializeField] [HiraCollectionDropdown(typeof(NPCDialogue))] private NPCDialogue next = null;
		public NPCDialogue Next => next;
	}
}