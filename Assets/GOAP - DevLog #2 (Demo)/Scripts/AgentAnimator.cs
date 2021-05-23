using System;

namespace UnityEngine.Internal
{
	public class AgentAnimator : MonoBehaviour
	{
		private const string animator_speed_variable = "Speed";
		private const string animator_inactive_variable = "Inactive";
		private const string animator_fighting_variable = "Fighting";
		private const string animator_action_variable = "Action";
		
		[SerializeField] private Animator animator = null;
		[SerializeField] private Transform selfTransform = null;

		private Vector3 _position = default;

		private static readonly int animator_speed = Animator.StringToHash(animator_speed_variable);
		private static readonly int animator_inactive = Animator.StringToHash(animator_inactive_variable);
		private static readonly int animator_fighting = Animator.StringToHash(animator_fighting_variable);
		private static readonly int animator_action = Animator.StringToHash(animator_action_variable);

		private void OnDestroy()
		{
			selfTransform = null;
			animator = null;
		}

		private void OnEnable()
		{
			_position = selfTransform.position;
			animator.SetBool(animator_inactive, false);
		}

		private void OnDisable()
		{
			animator.SetBool(animator_inactive, true);
			_position = selfTransform.position;
		}

		private void Update()
		{
			var deltaTime = Time.deltaTime;
			if (deltaTime <= 0) return;

			var currentPosition = selfTransform.position;

			var currentSpeed = (currentPosition - _position).magnitude / deltaTime;
			animator.SetFloat(animator_speed, currentSpeed);

			_position = currentPosition;
		}
	}
}