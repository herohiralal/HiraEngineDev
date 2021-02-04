using HiraEditor.HiraAssetEditorWindows;
using UnityEditor;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	[HiraAssetEditorWindow(typeof(DialogueTree))]
	public class DialogueGraph : EditorWindow, IHiraAssetEditorWindow
	{
		private DialogueGraphView _dialogueGraphView;
		private DialogueTree _target;
		private bool _initialized = false;

		private string _selectedGuid = "";

		public string SelectedGuid
		{
			get => _selectedGuid;
			set
			{
				_selectedGuid = value;
				_target = AssetDatabase.LoadAssetAtPath<DialogueTree>(AssetDatabase.GUIDToAssetPath(value));
				Enable();
			}
		}

		private void OnEnable()
		{
			if (_initialized) Enable();
		}

		private void Enable()
		{
			var root = rootVisualElement;
			_dialogueGraphView = new DialogueGraphView();
			_target.ConvertToNodes(_dialogueGraphView);
			_dialogueGraphView.StretchToParentSize();
			root.Add(_dialogueGraphView);

			_dialogueGraphView.OnSave -= OnSave;
			_dialogueGraphView.OnSave += OnSave;

			_dialogueGraphView.OnPing -= OnPing;
			_dialogueGraphView.OnPing += OnPing;
			
			_initialized = true;
		}

		private void OnSave()
		{
			_dialogueGraphView.ConvertToTree(_target);
		}

		private void OnPing()
		{
			EditorGUIUtility.PingObject(_target);
		}

		private void OnDisable()
		{
			var root = rootVisualElement;
			root.Remove(_dialogueGraphView);
			
			_dialogueGraphView.OnPing -= OnPing;
			_dialogueGraphView.OnSave -= OnSave;
			_dialogueGraphView = null;
		}
	}
}