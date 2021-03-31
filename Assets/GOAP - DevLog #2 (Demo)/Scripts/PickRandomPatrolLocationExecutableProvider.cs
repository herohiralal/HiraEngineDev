using HiraEngine.Components.AI;
using HiraEngine.Components.Blackboard;
using UnityEngine;

public class PickRandomPatrolLocationExecutable : Executable
{
	public PickRandomPatrolLocationExecutable Init(IBlackboardComponent blackboard, HiraBlackboardKey key)
	{
		_blackboard = blackboard;
		_key = key.Index;
		return this;
	}
	
	private IBlackboardComponent _blackboard;
	private ushort _key;
	private ExecutionStatus _result;
	
	public override void OnExecutionStart()
	{
        if (RandomLocation.TryGet(out var random))
        {
            _blackboard.SetValue<Vector3>(_key, random);
            _result = ExecutionStatus.Succeeded;
        }
        else _result = ExecutionStatus.Failed;
    }

	public override ExecutionStatus Execute(float deltaTime) => _result;

    public override void Dispose() => GenericPool<PickRandomPatrolLocationExecutable>.Return(this);
}

public class PickRandomPatrolLocationExecutableProvider : ScriptableObject, IExecutableProvider
{
	[HiraCollectionDropdown(typeof(VectorKey))]
	[SerializeField] private HiraBlackboardKey storeIn = null;
	
	public Executable GetExecutable(HiraComponentContainer target, IBlackboardComponent blackboard) =>
		GenericPool<PickRandomPatrolLocationExecutable>.Retrieve().Init(blackboard, storeIn);
}