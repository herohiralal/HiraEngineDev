using UnityEditor.Experimental.GraphView;

namespace Graphview.Scripts.Editor
{
	public class NPCDialogueNode : Node
	{
		public Port Input = null;
		public Port[] Responses = null;
	}
}