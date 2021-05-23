using UnityEngine.Scripting;

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

		[SerializeField] private Collider rightKickCollider = null;

		private Vector3 _position = default;

		private static readonly int animator_speed = Animator.StringToHash(animator_speed_variable);
		private static readonly int animator_inactive = Animator.StringToHash(animator_inactive_variable);
		private static readonly int animator_fighting = Animator.StringToHash(animator_fighting_variable);
		private static readonly int animator_action = Animator.StringToHash(animator_action_variable);

		public float Speed
		{
			get => animator.GetFloat(animator_speed);
			set => animator.SetFloat(animator_speed, value);
		}

		public bool Active
		{
			get => !animator.GetBool(animator_inactive);
			set => animator.SetBool(animator_inactive, !value);
		}

		public bool Fighting
		{
			get => animator.GetBool(animator_fighting);
			set => animator.SetBool(animator_fighting, value);
		}

		public int Action
		{
			get => animator.GetInteger(animator_action);
			set => animator.SetInteger(animator_action, value);
		}

		public bool IsKicking => Action == 1;
		public void Kick() => Action = 1;

		private void OnDestroy()
		{
			selfTransform = null;
			animator = null;
		}

		private void OnEnable()
		{
			_position = selfTransform.position;
			rightKickCollider.enabled = false;
			Active = true;
		}

		private void OnDisable()
		{
			Active = false;
			rightKickCollider.enabled = false;
			_position = selfTransform.position;
		}

		private void Update()
		{
			var deltaTime = Time.deltaTime;
			if (deltaTime <= 0) return;

			var currentPosition = selfTransform.position;

			Speed = (currentPosition - _position).magnitude / deltaTime;

			_position = currentPosition;
		}

		[Preserve]
		public void RightKick(int state)
		{
			rightKickCollider.enabled = state switch
			{
				1 => true,
				0 => false,
				_ => rightKickCollider.enabled
			};
		}
	}
}