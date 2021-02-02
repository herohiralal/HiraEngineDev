using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueGraph : EditorWindow
	{
		private DialogueGraphView _dialogueGraphView;
		private DialogueTree _target;
		private bool _initialized = false;

		[OnOpenAsset]
		private static bool OpenDialogueTree(int instanceID, int line)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceID);
			return AttemptOpen(obj);
		}

		public static bool AttemptOpen(Object tree)
		{
			if (!(tree is DialogueTree dt)) return false;
			
			var window = GetWindow<DialogueGraph>();
			window.titleContent = new GUIContent(dt.name);
			window._target = dt;
			window.Enable();

			return true;
		}

		private void OnEnable()
		{
			if (_initialized) Enable();
		}

		private void Enable()
		{
			var root = rootVisualElement;
			_dialogueGraphView = new DialogueGraphView(_target.ConvertToNodes());
			_dialogueGraphView.StretchToParentSize();
			root.Add(_dialogueGraphView);
		}

		private void OnDisable()
		{
			var root = rootVisualElement;
			root.Remove(_dialogueGraphView);
			_dialogueGraphView = null;
		}
	}
}