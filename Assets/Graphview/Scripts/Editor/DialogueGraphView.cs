using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueGraphView : GraphView
	{
		private DialogueNode[] _nodes;
		private ContentDragger _contentDragger;
		private SelectionDragger _selectionDragger;
		private RectangleSelector _rectangleSelector;
		private Edge[] _edges;
		private IMGUIContainer _toolbar;

		public event Action OnSave = delegate { };
		public event Action OnPing = delegate { };

		public DialogueGraphView((DialogueNode[], Edge[]) nodes)
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
			
					GUILayout.Label("Made by Rohan Jadav", EditorStyles.toolbarButton);
				}
				GUILayout.EndHorizontal();
			});
			Add(_toolbar);
			
			(_nodes, _edges) = nodes;
			foreach (var npcDialogueNode in _nodes)
			{
				AddElement(npcDialogueNode);
				npcDialogueNode.RefreshExpandedState();
				npcDialogueNode.RefreshPorts();
			}
			
			foreach (var edge in _edges) AddElement(edge);
		}

		~DialogueGraphView()
		{
			foreach (var edge in _edges) RemoveElement(edge);
			foreach (var npcDialogueNode in _nodes) RemoveElement(npcDialogueNode);
			_nodes = null;

			Remove(_toolbar);

			this.RemoveManipulator(_rectangleSelector);
			_rectangleSelector = null;
			this.RemoveManipulator(_selectionDragger);
			_selectionDragger = null;
			this.RemoveManipulator(_contentDragger);
			_contentDragger = null;
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			ports.ForEach(port =>
			{
				if (startPort != port && startPort.node != port.node && startPort.portType == port.portType)
					compatiblePorts.Add(port);
			});
			return compatiblePorts;
		}
	}
}