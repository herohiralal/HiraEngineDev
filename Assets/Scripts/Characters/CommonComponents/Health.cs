using System;
using System.Collections;
using UnityEngine.Events;

namespace UnityEngine.Internal
{
	public class Health : MonoBehaviour
	{
		private static readonly int shader_flash = Shader.PropertyToID("_Flash");

		[SerializeField] private Renderer effectMesh = null;
		[SerializeField] private UnityEvent<float> onDamage = null;

		private bool _damaged = false;

		public void AttemptApplyDamage(float value)
		{
			if (!_damaged && effectMesh != null) StartCoroutine(HealthDamageCoroutine());

			try
			{
				onDamage.Invoke(value);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
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