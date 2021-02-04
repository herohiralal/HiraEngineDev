using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Graphview.Scripts.Editor
{
	public static class ConversionUtility
	{
		public static (DialogueNode[], Edge[]) ConvertToNodes(this DialogueTree tree)
		{
			var correspondence = new Dictionary<Dialogue, int>();

			var npcDialogues = tree.Collection1;
			var length = npcDialogues.Length;
			var output = new DialogueNode[length + 1];
			var edges = new List<Edge>();
			
			// ENTRY port
			output[length] = EntryNode;
			
			for (var i = 0; i < length; i++)
			{
				var npcDialogue = npcDialogues[i];
				correspondence.Add(npcDialogue, i);

				var node = new DialogueNode
				{
					title = npcDialogue.name
				};
				var position = npcDialogue.position;
				position.height = Mathf.Max(position.height, 150);
				position.width = Mathf.Max(position.width, 200);
				node.SetPosition(position);

				var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Dialogue));
				inputPort.portName = "Input";
				node.inputContainer.Add(inputPort);
				node.Input = inputPort;

				var responses = npcDialogue.Collection1;
				var responseCount = responses.Length;
				var ports = new Port[responseCount];

				for (var j = 0; j < responseCount; j++)
				{
					var response = responses[j];

					var port = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
					port.portName = response.name;
					node.outputContainer.Add(port);

					ports[j] = port;
				}

				node.Responses = ports;

				output[i] = node;
			}

			if (length > 0)
			{
				var edge = output[length].Responses[0].ConnectTo(output[0].Input);
				edges.Add(edge);
			}
			
			for (var i = 0; i < length; i++)
			{
				var node = output[i];

				var responses = npcDialogues[i].Collection1;
				var responseCount = responses.Length;
				
				for (var j = 0; j < responseCount; j++)
				{
					var response = responses[j];
					var next = response.Next;
					if (next != null)
					{
						var index = correspondence[next];
						var nextNode = output[index];
						var edge = node.Responses[j].ConnectTo(nextNode.Input);
						edges.Add(edge);
					}
				}
			}

			return (output, edges.ToArray());
		}

		private static DialogueNode EntryNode
		{
			get
			{
				var entryNode = new DialogueNode
				{
					title = "ENTRY"
				};
				entryNode.SetPosition(new Rect(0, 0, 100, 150));
				var entryPort = entryNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
				entryPort.portName = "Start";
				entryNode.outputContainer.Add(entryPort);
				entryNode.Responses = new[] {entryPort};
				return entryNode;
			}
		}
	}
}