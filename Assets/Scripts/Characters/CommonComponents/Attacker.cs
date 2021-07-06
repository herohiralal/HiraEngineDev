using System.Collections.Generic;

namespace UnityEngine.Internal
{
	public class Attacker : MonoBehaviour
	{
		[SerializeField] public List<Health> ignoredHealthComponents = null;
		[SerializeField] private float damage = 5f;

		private void OnTriggerEnter(Collider other)
		{
			var otherHealth = other.GetComponent<Health>();
			if (otherHealth != null && !ignoredHealthComponents.Contains(otherHealth))
				otherHealth.AttemptApplyDamage(damage);
		}
	}
}