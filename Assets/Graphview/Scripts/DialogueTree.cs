using UnityEngine;

namespace Graphview.Scripts
{
	[System.Serializable]
	public class Dialogue
	{
		public string text = "";
		public int[] responses = { };
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
		[HideInInspector] public Rect position = default;
#endif
	}

	[System.Serializable]
	public class Response
	{
		public string text = "";
		public int nextDialogue = -1;
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
		[HideInInspector] public Rect position = default;
#endif
	}
	
	[CreateAssetMenu]
	public class DialogueTree : ScriptableObject
	{
		public int startIndex = -1;
		public Dialogue[] dialogues = { };
		public Response[] responses = { };
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
		[HideInInspector] public Rect entryNodePosition = default;
#endif
	}
}