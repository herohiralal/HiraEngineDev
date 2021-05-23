namespace UnityEngine.Internal
{
	public class PlayerProximityReproachSensor : MonoBehaviour
	{
		[SerializeField] private PlayerProximitySensor mainSensor = null;

		private void OnEnable()
		{
			HiraCommandBuffer.Instance.SetTimer(() => GetComponent<Collider>().enabled = true, 0.5f);
		}

		private void OnDisable()
		{
			GetComponent<Collider>().enabled = false;
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
				mainSensor.ReportExit();
		}
	}
}