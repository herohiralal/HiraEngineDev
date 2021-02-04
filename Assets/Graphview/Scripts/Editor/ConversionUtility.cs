using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Graphview.Scripts.Editor
{
	public static class ConversionUtility
	{
		public static void ConvertToNodes(this DialogueTree tree, out List<DialogueNode> dialogueNodes, out List<ResponseNode> responseNodes, out List<Edge> edges)
		{
			var dialogueCount = tree.dialogues.Length;
			var responseCount = tree.responses.Length;
			
			dialogueNodes = new List<DialogueNode>(dialogueCount + 1);
			responseNodes = new List<ResponseNode>(responseCount);
			edges = new List<Edge>();

			var entryNode = new DialogueNode();
			tree.entryNodePosition.PreprocessPositionRect();
			entryNode.SetPosition(tree.entryNodePosition);
			dialogueNodes.Add(entryNode);
			
			for (var i = 0; i < dialogueCount; i++)
			{
				var currentDialogue = tree.dialogues[i];
				var currentDialogueResponseCount = currentDialogue.responses.Length;
				var node = new DialogueNode(currentDialogueResponseCount)
				{
					title = currentDialogue.text,
				};
				
				currentDialogue.position.PreprocessPositionRect();
				node.SetPosition(currentDialogue.position);

				dialogueNodes.Add(node);
			}
			
			for (var i = 0; i < responseCount; i++)
			{
				var currentResponse = tree.responses[i];
				var node = new ResponseNode
				{
					title = currentResponse.text
				};

				currentResponse.position.PreprocessPositionRect();
				node.SetPosition(currentResponse.position);
				
				// connections
				var next = currentResponse.nextDialogue;
				if (next >= 0 && next < dialogueCount)
				{
					edges.Add(node.Next.ConnectTo(dialogueNodes[next].Input));
				}

				responseNodes.Add(node);
			}
			
			for (var i = 0; i < dialogueCount; i++)
			{
				var currentDialogue = tree.dialogues[i];
				var currentNode = dialogueNodes[i];
				
				var currentDialogueResponseCount = currentDialogue.responses.Length;
				for (var j = 0; j < currentDialogueResponseCount; j++)
				{
					var targetResponse = responseNodes[currentDialogue.responses[j]];
					edges.Add(currentNode.Responses[j].ConnectTo(targetResponse.Input));
				}
			}
		}

		private static void PreprocessPositionRect(this ref Rect position)
		{
			position.height = Mathf.Max(position.height, 150);
			position.width = Mathf.Max(position.width, 200);
		}
	}
}