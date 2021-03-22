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
	}
}