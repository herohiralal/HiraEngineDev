using System;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine;

namespace GraphTools.Behaviour.Editor.Nodes
{
	[Serializable]
	[SearcherItem(typeof(BehaviourStencil), SearcherContext.Graph, "Decorator")]
	public class BehaviourDecoratorNodeModel : NodeModel
	{
		public BehaviourDecoratorNodeModel()
		{
			Color = new Color(0.2f, 0.1f, 0.8f, 0.2f);
		}

		protected override void OnDefineNode()
		{
			base.OnDefineNode();

			this.AddExecutionInputPort("", orientation: Orientation.Vertical);
			this.AddExecutionOutputPort("", orientation: Orientation.Vertical);
		}
	}
}