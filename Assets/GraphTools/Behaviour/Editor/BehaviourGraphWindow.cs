using System.Collections.Generic;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;

namespace GraphTools.Behaviour.Editor
{
	public class BehaviourGraphWindow : GraphViewEditorWindow
	{
		[InitializeOnLoadMethod]
		private static void RegisterTool() =>
			ShortcutHelper.RegisterDefaultShortcuts<BehaviourGraphWindow>(BehaviourStencil.TOOL_NAME);

		public static void ShowBehaviourGraphWindow()
		{
			FindOrCreateGraphWindow<BehaviourGraphWindow>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			EditorToolName = "Behaviour";
		}

		protected override GraphView CreateGraphView()
		{
			return new BehaviourGraphView(this, CommandDispatcher);
		}

		protected override BlankPage CreateBlankPage()
		{
			var providers = new List<OnboardingProvider> {new BehaviourOnboardingProvider()};
			return new BlankPage(CommandDispatcher, providers);
		}
	}
}