using System.Collections;
using UnityEngine.AI;

namespace UnityEngine.Internal
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private Transform selfTransform = null;
		[SerializeField] private float speed = 3.5f;
		[SerializeField] private AgentAnimator animator = null;

		private bool _fighting = false;
		private bool _kicking = false;

		private Transform _mainCam = null;
		private Vector3 _startingCameraLocation = default;
		private Quaternion _startingCameraRotation = default;

		private void Awake()
		{
			// ReSharper disable once PossibleNullReferenceException
			_mainCam = Camera.main.transform;
			_startingCameraLocation = _mainCam.position;
			_startingCameraRotation = _mainCam.rotation;

			_mainCam.position = transform.position + new Vector3(0, 3, -4);
			_mainCam.eulerAngles = new Vector3(27, 0, 0);
		}

		private void OnDestroy()
		{
			_mainCam.position = _startingCameraLocation;
			_mainCam.rotation = _startingCameraRotation;
			_mainCam = null;
			
			selfTransform = null;
			animator = null;
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.LeftShift) || _kicking)
			{
				if (!_fighting)
				{
					_fighting = true;
					animator.Fighting = true;
				}

				if (!_kicking && Input.GetKeyDown(KeyCode.F))
				{
					StartCoroutine(KickCoroutine());
				}
			}
			else
			{
				if (_fighting)
				{
					_fighting = false;
					animator.Fighting = false;
				}

				var startingPosition = selfTransform.position;

				var intendedDirection = new Vector3(
					Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
					0,
					Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0);

				if (intendedDirection != Vector3.zero)
				{
					var intendedPosition = startingPosition + (intendedDirection.normalized * (Time.deltaTime * speed));
					if (NavMesh.SamplePosition(intendedPosition, out var hit, 0.3f, NavMesh.AllAreas))
					{
						selfTransform.position = hit.position;

						_mainCam.position = hit.position + new Vector3(0, 3, -4);

						var newYaw = Quaternion.LookRotation(intendedPosition - startingPosition, Vector3.up).eulerAngles.y;
						selfTransform.eulerAngles = new Vector3(0, newYaw, 0);
					}
				}
			}
		}

		private IEnumerator KickCoroutine()
		{
			_kicking = true;
			animator.Action = 1;
			yield return null;
			yield return null;
			animator.Action = 0;
			_kicking = false;
		}
	}
}