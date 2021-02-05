using UnityEditor.Experimental.GraphView;

namespace Graphview.Scripts.Editor
{
	public class EntryNode : Node
	{
		public EntryNode()
		{
			capabilities &= ~Capabilities.Copiable & ~Capabilities.Deletable & ~Capabilities.Copiable & ~Capabilities.Renamable & ~Capabilities.Selectable & ~Capabilities.Movable;
			
			base.title = "Entry";
			Start = base.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(Dialogue));
			Start.portName = "Start";
			outputContainer.Add(Start);
		}
		
		public readonly Port Start = null;
	}
}