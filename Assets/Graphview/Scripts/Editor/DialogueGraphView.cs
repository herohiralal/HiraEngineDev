using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueGraphView : GraphView
	{
		private List<DialogueNode> _dialogues;
		private List<ResponseNode> _responses;
		private List<Edge> _edges;
		private ContentDragger _contentDragger;
		private SelectionDragger _selectionDragger;
		private RectangleSelector _rectangleSelector;
		private IMGUIContainer _toolbar;

		public event Action OnSave = delegate { };
		public event Action OnPing = delegate { };

		public DialogueGraphView(List<DialogueNode> dialogues, List<ResponseNode> responses, List<Edge> edges)
		{
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

					if (GUILayout.Button("Add Dialogue", EditorStyles.toolbarButton)) CreateDialogueNode();
					if (GUILayout.Button("Add Response", EditorStyles.toolbarButton)) CreateResponseNode();

					GUILayout.Label("Made by Rohan Jadav", EditorStyles.toolbarButton);
				}
				GUILayout.EndHorizontal();
			});
			Add(_toolbar);

			(_dialogues, _responses, _edges) = (dialogues, responses, edges);

			foreach (var node in dialogues)
			{
				AddElement(node);
				node.RefreshExpandedState();
				node.RefreshPorts();
			}

			foreach (var node in _responses)
			{
				AddElement(node);
				node.RefreshExpandedState();
				node.RefreshPorts();
			}

			foreach (var edge in _edges) AddElement(edge);
		}

		~DialogueGraphView()
		{
			foreach (var edge in _edges) RemoveElement(edge);
			foreach (var node in _dialogues) RemoveElement(node);
			foreach (var node in _responses) RemoveElement(node);
			(_dialogues, _responses, _edges) = (null, null, null);

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

		private void CreateDialogueNode()
		{
			var dialogueNode = new DialogueNode(0) {title = "Dialogue baby dialogue!"};
			dialogueNode.SetPosition(new Rect(0, 0, 150, 200));
			_dialogues.Add(dialogueNode);
			AddElement(dialogueNode);
		}

		private void CreateResponseNode()
		{
			var responseNode = new ResponseNode {title = "Response baby response!"};
			responseNode.SetPosition(new Rect(0, 0, 150, 200));
			_responses.Add(responseNode);
			AddElement(responseNode);
		}
	}
}