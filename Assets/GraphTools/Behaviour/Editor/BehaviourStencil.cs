using JetBrains.Annotations;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace GraphTools.Behaviour.Editor
{
	public class BehaviourStencil : Stencil
	{
		[UsedImplicitly]
		public BehaviourStencil()
		{
		}

		public override IGraphProcessingErrorModel CreateProcessingErrorModel(GraphProcessingError error)
		{
			return error.SourceNode is {Destroyed: false} ? new GraphProcessingErrorModel(error) : null;
		}

		public const string GRAPH_NAME = "Behaviour";
		public const string TOOL_NAME = "Behaviour Editor";
		public override string ToolName => TOOL_NAME;

		public static TypeHandle BehaviourNodeType { get; } = TypeSerializer.GenerateCustomTypeHandle("BehaviourNode");
	}
}