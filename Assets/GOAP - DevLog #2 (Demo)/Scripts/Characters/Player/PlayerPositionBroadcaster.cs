using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class PlayerPositionBroadcaster : MonoBehaviour
	{
		[SerializeField] private Transform target = null;
		[SerializeField] private HiraBlackboardTemplate blackboard = null;
		[HiraCollectionDropdown(typeof(VectorKey))] [SerializeField] private HiraBlackboardKey key = null;

		private void Reset()
		{
			target = transform;
		}

		private void Update()
		{
			var index = key.Index;
			var position = target.position;

			blackboard.UpdateInstanceSyncedKey(index, position);
		}
	}
}