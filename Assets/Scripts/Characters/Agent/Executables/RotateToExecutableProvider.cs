using HiraEngine.Components.AI;
using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class RotateToExecutable : Executable, IPoolReturnCallbackReceiver
	{
		public RotateToExecutable Init(Transform target, IBlackboardComponent blackboard, HiraBlackboardKey locationKey)
		{
			_target = target;
			_blackboard = blackboard;
			_locationKey = locationKey;
			return this;
		}
		
		private Transform _target;
		private IBlackboardComponent _blackboard;
		private HiraBlackboardKey _locationKey;
		
		public override ExecutionStatus Execute(float deltaTime)
		{
			var location = _blackboard.GetValue<Vector3>(_locationKey.Index);
			_target.eulerAngles = new Vector3(
				0,
				Quaternion.LookRotation(location - _target.position, Vector3.up).eulerAngles.y,
				0);
			return ExecutionStatus.Succeeded;
		}

		public override void Dispose()
		{
			GenericPool<RotateToExecutable>.Return(this);
		}
		
		public void OnReturn()
		{
			_target = null;
			_blackboard = null;
			_locationKey = null;
		}
	}
	
	public class RotateToExecutableProvider : ScriptableObject, IExecutableProvider
	{
		[HiraCollectionDropdown(typeof(VectorKey))] [SerializeField] private HiraBlackboardKey location = null;

		public Executable GetExecutable(HiraComponentContainer target, IBlackboardComponent blackboard)
		{
			return GenericPool<RotateToExecutable>.Retrieve().Init(target.transform, blackboard, location);
		}
	}
}