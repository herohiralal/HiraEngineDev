using UnityEngine;

namespace LGOAPDemo
{
	public class IntegerKey : BlackboardKey
	{
		[SerializeField] private int defaultValue = 0;
		public override byte SizeInBytes => 4;

		public override unsafe void SetDefault(byte* value)
		{
			*((int*) value) = defaultValue;
		}
	}
}