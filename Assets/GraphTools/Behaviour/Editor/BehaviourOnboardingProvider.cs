using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;
using UnityEngine.UIElements;

namespace GraphTools.Behaviour.Editor
{
	class BehaviourOnboardingProvider : OnboardingProvider
	{
		public override VisualElement CreateOnboardingElements(CommandDispatcher commandDispatcher)
		{
			var template = new GraphTemplate<BehaviourStencil>(BehaviourStencil.GRAPH_NAME);
			return AddNewGraphButton<BehaviourGraphAssetModel>(template);
		}
	}
}