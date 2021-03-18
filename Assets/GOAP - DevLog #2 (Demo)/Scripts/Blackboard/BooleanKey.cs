using UnityEngine;

namespace LGOAPDemo
{
	public class BooleanKey : BlackboardKey
	{
		[SerializeField] private bool defaultValue = false;
		public override byte SizeInBytes => 1;

		public override unsafe void SetDefault(byte* value)
		{
			*value = (byte) (defaultValue ? 1 : 0);
		}
	}
}