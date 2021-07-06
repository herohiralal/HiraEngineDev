namespace UnityEngine.Internal
{
	public class AgentAnimatedAction : StateMachineBehaviour
	{
		private static readonly int animator_action = Animator.StringToHash("Action");
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
			animator.SetInteger(animator_action, 0);
	}
}