using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class PlayerPositionBroadcaster : MonoBehaviour
	{
		[SerializeField] private Transform target = null;
		[SerializeField] private HiraBlackboardTemplate blackboard = null;
		[HiraCollectionDropdown(typeof(VectorKey))] [SerializeField] private HiraBlackboardKey key = null;

		private void Reset() => target = transform;

		private void OnDisable() => blackboard.UpdateInstanceSyncedKey<Vector3>(key.Index, new Vector3(0, 100, 0));

		private void Update() => blackboard.UpdateInstanceSyncedKey<Vector3>(key.Index, target.position);
	}
}