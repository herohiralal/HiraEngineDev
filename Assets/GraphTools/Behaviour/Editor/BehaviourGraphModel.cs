using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace GraphTools.Behaviour.Editor
{
	public class BehaviourGraphModel : GraphModel
	{
		protected override bool IsCompatiblePort(IPortModel startPortModel, IPortModel compatiblePortModel)
		{
			return
				startPortModel.DataTypeHandle == compatiblePortModel.DataTypeHandle // matching datatype
				&& startPortModel.Direction != compatiblePortModel.Direction // opposite direction
				&& startPortModel.Orientation == compatiblePortModel.Orientation // matching orientation
				&& startPortModel.NodeModel != compatiblePortModel.NodeModel; // not the same node
		}
	}
}