using UnityEditor.Experimental.GraphView;

namespace Graphview.Scripts.Editor
{
	public class ResponseNode : Node
	{
		public ResponseNode()
		{
			capabilities |= Capabilities.Renamable;
			
			Input = base.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Response));
			Input.portName = "Response";
			inputContainer.Add(Input);

			Next = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
			Next.portName = "Next";
			outputContainer.Add(Next);
		}
		
		public readonly Port Input = null;
		public readonly Port Next = null;
	}
}