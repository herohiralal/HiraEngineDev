using JetBrains.Annotations;
using UnityEngine;

namespace DefaultNamespace
{
	[HiraConsole]
	public static class SomeClass
	{
		[HiraConsole, UsedImplicitly]
		private static void Add(float a, float b) => Debug.Log(a + b);

		[HiraConsole, UsedImplicitly]
		private static void Subtract(float a, float b) => Debug.Log(a - b);

		[HiraConsole, UsedImplicitly]
		private static void Multiply(float a, float b) => Debug.Log(a * b);

		[HiraConsole, UsedImplicitly]
		private static void Divide(float a, float b) => Debug.Log(a / b);

		[HiraConsole, UsedImplicitly]
		private static void Power(float a, float b) => Debug.Log(Mathf.Pow(a, b));
	}
}