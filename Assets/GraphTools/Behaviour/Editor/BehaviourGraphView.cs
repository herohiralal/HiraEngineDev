using UnityEditor.GraphToolsFoundation.Overdrive;

namespace GraphTools.Behaviour.Editor
{
	public class BehaviourGraphView : GraphView
	{
		public BehaviourGraphView(GraphViewEditorWindow window, CommandDispatcher commandDispatcher, string uniqueGraphViewName = null) : base(window, commandDispatcher, uniqueGraphViewName)
		{
		}
	}
}