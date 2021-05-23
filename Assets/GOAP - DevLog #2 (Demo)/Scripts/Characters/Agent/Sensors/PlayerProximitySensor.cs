using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class PlayerProximitySensor : MonoBehaviour
	{
		[SerializeField] private HiraBlackboard blackboard = null;
		[HiraCollectionDropdown(typeof(BooleanKey))] [SerializeField] private HiraBlackboardKey key = null;

		public void ReportEntry() => blackboard.SetValue<byte>(key.Index, 1);

		public void ReportExit() => blackboard.SetValue<byte>(key.Index, 0);
	}
}