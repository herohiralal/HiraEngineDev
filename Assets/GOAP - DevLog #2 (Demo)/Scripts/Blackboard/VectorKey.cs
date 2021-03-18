using UnityEngine;

namespace LGOAPDemo
{
	public class VectorKey : BlackboardKey
	{
		[SerializeField] private Vector3 defaultValue = Vector3.zero;
		public override unsafe byte SizeInBytes => (byte) sizeof(Vector3);

		public override unsafe void SetDefault(byte* value)
		{
			*((Vector3*) value) = defaultValue;
		}
	}
}