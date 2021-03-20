using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Internal
{
	public class BlackboardTester : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardTemplate template = null;
		[SerializeField] private HiraBlackboard blackboard = null;
		[SerializeField] private EffectorTester effect = null;

		private void Awake()
		{
			object init = null;
			template.Initialize(ref init);
			blackboard.Initialize(ref init);
		}

		private void OnDestroy()
		{
			blackboard.Shutdown();
			template.Shutdown();
		}

		private unsafe void OnValidate()
		{
			if (effect != null)
			{
				var blackboardPtr = (byte*) blackboard.Data.GetUnsafePtr();
				foreach (var effector in effect.Collection1)
				{
					var memory = new byte[effector.MemorySize];
					fixed (byte* memoryPtr = memory)
					{
						effector.AppendMemory(memoryPtr);
						effector.Function.Invoke(blackboardPtr, memoryPtr);
					}
				}

				effect = null;
			}
		}
	}
}