using HiraEngine.Components.AI.LGOAP;

namespace UnityEngine.Internal
{
	[System.Serializable, DynamicallyAccessibleEnum("6e8da6b9-998f-4186-81e4-7a9d11fc1ebd"), System.Flags]
	public enum Class : byte
	{
		None = 0, Warrior = 1 << 0, Mage = 1 << 1, Rogue = 1 << 2
	}

	public class Initializer : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardTemplate template = null;
		[SerializeField] private HiraBlackboardComponent blackboard = null;
		[SerializeField] private GoalOrientedActionPlannerDomain domain = null;

		private void Awake()
		{
			object init = null;
			template.Initialize(ref init);
			blackboard.Initialize(ref init);
			domain.Initialize(ref init);
		}

		private void OnDestroy()
		{
			domain.Shutdown();
			blackboard.Shutdown();
			template.Shutdown();
		}
	}
}