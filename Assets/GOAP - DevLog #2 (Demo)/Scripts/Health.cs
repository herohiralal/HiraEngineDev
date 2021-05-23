using System.Collections;

namespace UnityEngine.Internal
{
	public class Health : MonoBehaviour
	{
		private static readonly int shader_flash = Shader.PropertyToID("_Flash");

		[SerializeField] private Renderer effectMesh = null;

		private bool _damaged = false;

		public void AttemptApplyDamage(float value)
		{
			if (!_damaged) StartCoroutine(HealthDamageCoroutine());
		}

		private IEnumerator HealthDamageCoroutine()
		{
			_damaged = true;
			effectMesh.material.SetFloat(shader_flash, 1);
			yield return new WaitForSeconds(0.1f);
			effectMesh.material.SetFloat(shader_flash, 0);
			_damaged = false;
		}
	}
}