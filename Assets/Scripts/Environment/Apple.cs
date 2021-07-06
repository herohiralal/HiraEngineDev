using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class Apple : MonoBehaviour
	{
		[HiraCollectionDropdown(typeof(VectorKey))] [SerializeField] private HiraBlackboardKey targetLocationIntentionKey = null;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Agent")) return;

			var blackboard = other.GetComponentInParent<IBlackboardComponent>();
			if (blackboard != null && blackboard.GetValue<Vector3>(targetLocationIntentionKey.Index) == transform.position)
			{
				gameObject.SetActive(false);
			}
		}
	}
}