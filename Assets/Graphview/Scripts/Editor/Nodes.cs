using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Graphview.Scripts.Editor
{
	public class EntryNode : Node
	{
		public EntryNode()
		{
			capabilities &= ~Capabilities.Copiable & ~Capabilities.Deletable & ~Capabilities.Copiable & ~Capabilities.Renamable & ~Capabilities.Selectable;
			
			base.title = "Entry";
			Start = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
			Start.portName = "Start";
			outputContainer.Add(Start);
		}
		
		public readonly Port Start = null;
	}
	
	public class DialogueNode : Node
	{
		public DialogueNode(int responseCount = 0)
		{
			capabilities |= Capabilities.Renamable;
			
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