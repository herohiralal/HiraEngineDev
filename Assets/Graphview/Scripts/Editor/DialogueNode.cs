using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
	public class DialogueNode : Node
	{
		public DialogueNode(int responseCount = 0)
		{
			capabilities |= Capabilities.Renamable;
			
			Input = base.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Dialogue));
			Input.portName = "Dialogue";
			inputContainer.Add(Input);
			
			Responses = new List<Port>(responseCount);
			
			for (var i = 0; i < responseCount; i++) AddOutputPort(i);

			_addChoiceButton = new Button(AddChoice) {text = "+"};
			titleContainer.Add(_addChoiceButton);
		}

		private void AddOutputPort(int i)
		{
			var outputPort = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Response));
			outputPort.portName = $"Choice {i + 1}";
			outputContainer.Add(outputPort);
			Responses.Add(outputPort);
		}

		private void AddChoice()
		{
			AddOutputPort(Responses.Count);
			
			RefreshExpandedState();
			RefreshPorts();
		}

		private readonly Button _addChoiceButton = null;
		public readonly Port Input = null;
		public readonly List<Port> Responses = null;
	}
}