using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Graphview.Scripts.Editor
{
	public static class ConversionUtility
	{
		public static void ConvertToNodes(this DialogueTree tree, DialogueGraphView graphView)
		{
			var dialogueCount = tree.dialogues.Length;
			var responseCount = tree.responses.Length;
			
			var dialogueNodes = new List<DialogueNode>(dialogueCount);
			graphView.Dialogues = dialogueNodes;
			var responseNodes = new List<ResponseNode>(responseCount);
			graphView.Responses = responseNodes;

			// entry
			var entryNode = new EntryNode();
			graphView.EntryNode = entryNode;
			tree.entryNodePosition.PreprocessPositionRect();
			entryNode.SetPosition(tree.entryNodePosition);
			graphView.AddElement(entryNode);
			entryNode.RefreshExpandedState();
			entryNode.RefreshPorts();

			// dialogues
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

			// entry -> dialogue
			if (tree.startIndex >= 0 && tree.startIndex < dialogueCount)
				graphView.AddElement(entryNode.Start.ConnectTo(dialogueNodes[tree.startIndex].Input));
			
			// responses
			for (var i = 0; i < responseCount; i++)
			{
				var currentResponse = tree.responses[i];
				var currentNode = new ResponseNode
				{
					title = currentResponse.text
				};

				currentResponse.position.PreprocessPositionRect();
				currentNode.SetPosition(currentResponse.position);
				
				// response -> dialogue
				var next = currentResponse.nextDialogue;
				if (next >= 0 && next < dialogueCount)
					graphView.AddElement(currentNode.Next.ConnectTo(dialogueNodes[next].Input));

				responseNodes.Add(currentNode);
				
				graphView.AddElement(currentNode);
				currentNode.RefreshExpandedState();
				currentNode.RefreshPorts();
			}
			
			// dialogue -> response
			for (var i = 0; i < dialogueCount; i++)
			{
				var currentDialogue = tree.dialogues[i];
				var currentNode = dialogueNodes[i];
				
				var currentDialogueResponseCount = currentDialogue.responses.Length;
				for (var j = 0; j < currentDialogueResponseCount; j++)
				{
					var responseIndex = currentDialogue.responses[j];
					if (responseIndex >= 0 && responseIndex < responseCount)
						graphView.AddElement(currentNode.Responses[j].ConnectTo(responseNodes[responseIndex].Input));
				}

				graphView.AddElement(currentNode);
				currentNode.RefreshExpandedState();
				currentNode.RefreshPorts();
			}
		}

		private static void PreprocessPositionRect(this ref Rect position)
		{
			position.height = Mathf.Max(position.height, 150);
			position.width = Mathf.Max(position.width, 200);
		}

		public static void ConvertToTree(this DialogueGraphView graphView, DialogueTree tree)
		{
			tree.entryNodePosition = graphView.EntryNode.GetPosition();
			var firstDialogue = graphView.EntryNode.Start.connections.FirstOrDefault()?.input.node;
			tree.startIndex = firstDialogue != null ? graphView.Dialogues.FindIndex(n => n == firstDialogue) : -1;

			var dialogueNodes = graphView.Dialogues;
			var dialogueCount = dialogueNodes.Count;
			var responseNodes = graphView.Responses;
			var responseCount = responseNodes.Count;

			var dialogues = new Dialogue[dialogueCount];
			tree.dialogues = dialogues;
			var responses = new Response[responseCount];
			tree.responses = responses;

			for (var i = 0; i < responseCount; i++)
			{
				var currentNode = responseNodes[i];
				var currentResponse = new Response();
				responses[i] = currentResponse;

				currentResponse.text = currentNode.title;
				currentResponse.position = currentNode.GetPosition();
				
				// connections
				var nextDialogue = currentNode.Next.connections.FirstOrDefault()?.input.node;
				currentResponse.nextDialogue = nextDialogue != null ? graphView.Dialogues.FindIndex(n => n == nextDialogue) : -1;
			}
			
			for (var i = 0; i < dialogueCount; i++)
			{
				var currentNode = dialogueNodes[i];
				
				var currentDialogue = new Dialogue();
				dialogues[i] = currentDialogue;

				currentDialogue.text = currentNode.title;
				currentDialogue.position = currentNode.GetPosition();
				
				// connections
				var currentDialogueResponses = currentNode.Responses;
				var currentDialogueResponseCount = currentDialogueResponses.Count;
				
				currentDialogue.responses = new int[currentDialogueResponseCount];
				for (var j = 0; j < currentDialogueResponseCount; j++)
				{
					var responseNode = currentDialogueResponses[j].connections.FirstOrDefault()?.input.node;
					currentDialogue.responses[j] = responseNode != null ? graphView.Responses.FindIndex(n=>n==responseNode) : -1;
				}
			}
		}
	}
}