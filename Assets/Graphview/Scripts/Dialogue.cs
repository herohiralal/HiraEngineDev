using UnityEngine;

namespace Graphview.Scripts
{
	public class Dialogue : HiraCollection<Response>
	{
#if UNITY_EDITOR && !STRIP_EDITOR_CODE
#pragma warning disable 414
		private static readonly string collection1_name = "Responses";
#pragma warning restore 414

		[HideInInspector] public Rect position = default;
#endif
	}
}