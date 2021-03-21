using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Internal
{
	[System.Serializable, DynamicallyAccessibleEnum("6e8da6b9-998f-4186-81e4-7a9d11fc1ebd"), System.Flags]
	public enum Class : byte
	{
		None = 0, Warrior = 1 << 0, Mage = 1 << 1, Rogue = 1 << 2
	}

	public class BlackboardTester : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardTemplate template = null;
		[SerializeField] private HiraBlackboardComponent blackboard = null;
		[SerializeField] private HiraSharedBlackboard sharedBlackboard = null;
		[SerializeField] private EffectorTester effect = null;

		private void Awake()
		{
			object init = null;
			template.Initialize(ref init);
			blackboard.Initialize(ref init);
            sharedBlackboard.Initialize(ref init);
        }

		private void OnDestroy()
		{
            sharedBlackboard.Shutdown();
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