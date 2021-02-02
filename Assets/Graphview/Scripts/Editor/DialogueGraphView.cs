using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueGraphView : GraphView
	{
		private NPCDialogueNode[] _nodes;
		private ContentDragger _contentDragger;
		private SelectionDragger _selectionDragger;
		private RectangleSelector _rectangleSelector;
		private Edge[] _edges;

		public DialogueGraphView((NPCDialogueNode[], Edge[]) nodes)
		{
			(_nodes, _edges) = nodes;
			foreach (var npcDialogueNode in _nodes)
			{
				AddElement(npcDialogueNode);
				npcDialogueNode.RefreshExpandedState();
				npcDialogueNode.RefreshPorts();
			}
			
			foreach (var edge in _edges)
			{
				AddElement(edge);
			}

			_contentDragger = new ContentDragger();
			this.AddManipulator(_contentDragger);
			_selectionDragger = new SelectionDragger();
			this.AddManipulator(_selectionDragger);
			_rectangleSelector = new RectangleSelector();
			this.AddManipulator(_rectangleSelector);
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

		~DialogueGraphView()
		{
			foreach (var npcDialogueNode in _nodes) RemoveElement(npcDialogueNode);
			_nodes = null;

			this.RemoveManipulator(_rectangleSelector);
			_rectangleSelector = null;
			this.RemoveManipulator(_selectionDragger);
			_selectionDragger = null;
			this.RemoveManipulator(_contentDragger);
			_contentDragger = null;
		}
	}
}