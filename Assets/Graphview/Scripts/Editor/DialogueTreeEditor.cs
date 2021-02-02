using UnityEditor;
using UnityEngine;

namespace Graphview.Scripts.Editor
{
	[CustomEditor(typeof(DialogueTree))]
	public class DialogueTreeEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open"))
				if (!DialogueGraph.AttemptOpen(target))
					Debug.LogError("There was a problem opening the dialogue tree.");
		}
	}
}