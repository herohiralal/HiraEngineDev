using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace GraphTools.Behaviour.Editor
{
	public class BehaviourGraphAssetModel : GraphAssetModel
	{
		[MenuItem("Assets/Create/Behaviour", priority = 0)]
		private static void CreateBehaviour()
		{
			const string path = "Assets";
			var template = new GraphTemplate<BehaviourStencil>(BehaviourStencil.GRAPH_NAME);
			CommandDispatcher dispatcher = null;
			if (EditorWindow.HasOpenInstances<BehaviourGraphWindow>())
			{
				var window = EditorWindow.GetWindow<BehaviourGraphWindow>();
				if (window != null)
				{
					dispatcher = window.CommandDispatcher;
				}
			}

			GraphAssetCreationHelpers<BehaviourGraphAssetModel>.CreateInProjectWindow(template, dispatcher, path);
		}

		[OnOpenAsset(1)]
		private static bool OpenBehaviour(int instanceId, int line)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceId);
			if (!(obj is BehaviourGraphAssetModel)) return false;
			
			var path = AssetDatabase.GetAssetPath(instanceId);
			var asset = AssetDatabase.LoadAssetAtPath<BehaviourGraphAssetModel>(path);
			if (asset == null) return false;

			var window = GraphViewEditorWindow.FindOrCreateGraphWindow<BehaviourGraphWindow>();
			return window != null;
		}

		protected override Type GraphModelType => typeof(BehaviourGraphModel);
		public override IBlackboardGraphModel BlackboardGraphModel { get; }
		public BehaviourGraphAssetModel() => BlackboardGraphModel = new BehaviourBlackboardGraphModel();
	}
}