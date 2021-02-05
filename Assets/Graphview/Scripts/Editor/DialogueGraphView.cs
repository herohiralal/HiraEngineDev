using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueGraphView : GraphView
	{
		public EntryNode EntryNode;
		public List<DialogueNode> Dialogues;
		public List<ResponseNode> Responses;
		private ContentDragger _contentDragger;
		private SelectionDragger _selectionDragger;
		private RectangleSelector _rectangleSelector;
		private IMGUIContainer _toolbar;
		private readonly EditorWindow _parent = null;

		public event Action OnSave = delegate { };
		public event Action OnPing = delegate { };

		public DialogueGraphView(EditorWindow inParent)
		{
			_parent = inParent;
			
			_contentDragger = new ContentDragger();
			this.AddManipulator(_contentDragger);
			_selectionDragger = new SelectionDragger();
			this.AddManipulator(_selectionDragger);
			_rectangleSelector = new RectangleSelector();
			this.AddManipulator(_rectangleSelector);

			_toolbar = new IMGUIContainer(() =>
			{
				GUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					if (GUILayout.Button("Save", EditorStyles.toolbarButton)) OnSave.Invoke();
					GUILayout.Space(6);
					if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton)) OnPing.Invoke();

					GUILayout.FlexibleSpace();

					GUILayout.Label("Made by Rohan Jadav", EditorStyles.toolbarButton);
				}
				GUILayout.EndHorizontal();
			});
			Add(_toolbar);
		}

		~DialogueGraphView()
		{
			foreach (var node in Dialogues) RemoveElement(node);
			foreach (var node in Responses) RemoveElement(node);
			(Dialogues, Responses) = (null, null);

			RemoveElement(EntryNode);
			EntryNode = null;

			Remove(_toolbar);
			_toolbar = null;

			this.RemoveManipulator(_rectangleSelector);
			_rectangleSelector = null;
			this.RemoveManipulator(_selectionDragger);
			_selectionDragger = null;
			this.RemoveManipulator(_contentDragger);
			_contentDragger = null;
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList().Where(port =>
					port != startPort &&
					port.direction != startPort.direction &&
					port.node != startPort.node &&
					port.portType == startPort.portType)
				.ToList();
		}

		public void CreateDialogueNode(Vector2 position)
		{
			var dialogueNode = new DialogueNode {title = "Dialogue baby dialogue!"};
			dialogueNode.SetPosition(new Rect(position.x, position.y, 150, 200));
			Dialogues.Add(dialogueNode);
			AddElement(dialogueNode);
			dialogueNode.RefreshExpandedState();
			dialogueNode.RefreshPorts();
		}

		public void CreateResponseNode(Vector2 position)
		{
			var responseNode = new ResponseNode("Response baby response!");
			responseNode.SetPosition(new Rect(position.x, position.y, 150, 200));
			Responses.Add(responseNode);
			AddElement(responseNode);
			responseNode.RefreshExpandedState();
			responseNode.RefreshPorts();
		}
	}
}