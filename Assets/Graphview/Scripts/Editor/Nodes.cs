using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Graphview.Scripts.Editor
{
	public class DialogueNode : Node
	{
		public DialogueNode()
		{
			base.title = "Entry";
			Input = null;

			var entryPort = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
			entryPort.portName = "Start";
			outputContainer.Add(entryPort);
			Responses = new List<Port> {entryPort};
		}

		public DialogueNode(int responseCount)
		{
			Input = base.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Dialogue));
			Input.portName = "Dialogue";
			inputContainer.Add(Input);
			
			Responses = new List<Port>(responseCount);
			
			for (var i = 0; i < responseCount; i++)
			{
				var outputPort = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Response));
				outputPort.portName = $"Choice {i + 1}";
				outputContainer.Add(outputPort);
				Responses.Add(outputPort);
			}
		}
		
		public readonly Port Input = null;
		public readonly List<Port> Responses = null;
	}

	public class ResponseNode : Node
	{
		public ResponseNode()
		{
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