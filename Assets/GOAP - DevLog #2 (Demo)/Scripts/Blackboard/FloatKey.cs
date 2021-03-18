using UnityEngine;

namespace LGOAPDemo
{
	public class FloatKey : BlackboardKey
	{
		[SerializeField] private float defaultValue = 0f;
		public override byte SizeInBytes => 4;

		public override unsafe void SetDefault(byte* value)
		{
			*((float*) value) = defaultValue;
		}
	}
}