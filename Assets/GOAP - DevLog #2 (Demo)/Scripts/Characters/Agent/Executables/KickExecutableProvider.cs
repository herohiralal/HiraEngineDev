using HiraEngine.Components.AI;
using HiraEngine.Components.AI.Internal;

namespace UnityEngine.Internal
{
	public class KickExecutable : Executable, IPoolReturnCallbackReceiver
	{
		public KickExecutable Init(AgentAnimator animator)
		{
			_animator = animator;
			return this;
		}

		private AgentAnimator _animator = null;
		
		public override void OnExecutionStart() =>
			_animator.Kick();

		public override ExecutionStatus Execute(float deltaTime) =>
			_animator.IsKicking ? ExecutionStatus.InProgress : ExecutionStatus.Succeeded;

		public override void OnExecutionAbort()
		{
			_animator.Action = 0;
		}

		public override void Dispose()
		{
			GenericPool<KickExecutable>.Return(this);
		}

		public void OnReturn()
		{
			_animator = null;
		}
	}
	
	public class KickExecutableProvider : ScriptableObject, IExecutableProvider
	{
		public Executable GetExecutable(HiraComponentContainer target, IBlackboardComponent blackboard)
		{
			if (target is IContainsComponent<AgentAnimator> animatedTarget)
			{
				return GenericPool<KickExecutable>.Retrieve().Init(animatedTarget.Component);
			}
			
			return AutoFailExecutable.INSTANCE;
		}
	}
}