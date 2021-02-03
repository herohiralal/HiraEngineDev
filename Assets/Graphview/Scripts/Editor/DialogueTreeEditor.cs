using HiraEditor.HiraAssetEditorWindows;
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
				HiraAssetEditorWindowUtility.OnOpenHiraAsset(target.GetInstanceID(), 0);
		}
	}
}