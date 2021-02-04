using UnityEngine;

namespace Graphview.Scripts
{
	[CreateAssetMenu]
	public class DialogueTree : HiraCollection<Dialogue>
	{
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
#pragma warning disable 414
		private static readonly string collection1_name = "Dialogues";
#pragma warning restore 414
#endif
	}
}