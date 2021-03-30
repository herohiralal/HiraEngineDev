using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEditor.GraphToolsFoundation.Overdrive.BasicModel;

namespace GraphTools.Behaviour.Editor
{
	public class BehaviourBlackboardGraphModel : BlackboardGraphModel
	{
		public override string GetBlackboardTitle() => "Behaviour";

		public override IEnumerable<IVariableDeclarationModel> GetSectionRows(string sectionName) =>
			Enumerable.Empty<IVariableDeclarationModel>();

		public override void PopulateCreateMenu(string sectionName, GenericMenu menu, CommandDispatcher commandDispatcher)
		{
		}
	}
}