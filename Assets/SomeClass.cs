using JetBrains.Annotations;
using UnityEngine;

namespace DefaultNamespace
{
	[HiraConsoleType("Mathematics")]
	public static class SomeClass
	{
		[HiraConsoleCallable, UsedImplicitly]
		private static void Add(float a, float b) => Debug.Log(a + b);

		[HiraConsoleCallable, UsedImplicitly]
		private static void Subtract(float a, float b) => Debug.Log(a - b);

		[HiraConsoleCallable, UsedImplicitly]
		private static void Multiply(float a, float b) => Debug.Log(a * b);

		[HiraConsoleCallable, UsedImplicitly]
		private static void Divide(float a, float b) => Debug.Log(a / b);

		[HiraConsoleCallable, UsedImplicitly]
		private static void Power(float a, float b) => Debug.Log(Mathf.Pow(a, b));
	}
}