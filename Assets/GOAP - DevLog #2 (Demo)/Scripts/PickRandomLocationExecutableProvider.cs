using HiraEngine.Components.AI;
using HiraEngine.Components.Blackboard;
using UnityEngine;

public class PickRandomLocationExecutable : Executable
{
	public PickRandomLocationExecutable Init(IBlackboardComponent blackboard, HiraBlackboardKey key, string locationName)
	{
		_blackboard = blackboard;
		_key = key.Index;
		_locationName = locationName;
		return this;
	}
	
	private string _locationName;
	private IBlackboardComponent _blackboard;
	private ushort _key;
	private ExecutionStatus _result;
	private RandomLocation _current;
	
	public override void OnExecutionStart()
	{
        if (RandomLocation.TryGet(_locationName, out _current))
        {
            _blackboard.SetValue<Vector3>(_key, _current.transform.position);
            _result = ExecutionStatus.Succeeded;
        }
        else _result = ExecutionStatus.Failed;

        _blackboard = null;
    }

	public override ExecutionStatus Execute(float deltaTime) => _result;

    public override void Dispose()
    {
	    _current = null;
	    GenericPool<PickRandomLocationExecutable>.Return(this);
    }
}

public class PickRandomLocationExecutableProvider : ScriptableObject, IExecutableProvider
{
	[HiraCollectionDropdown(typeof(VectorKey))]
	[SerializeField] private HiraBlackboardKey storeIn = null;
	[SerializeField] private string location = "";
	
	public Executable GetExecutable(HiraComponentContainer target, IBlackboardComponent blackboard) =>
		GenericPool<PickRandomLocationExecutable>.Retrieve().Init(blackboard, storeIn, location);
}