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
	private RandomLocation _current;

    public override ExecutionStatus Execute(float deltaTime)
    {
        ExecutionStatus result;
        if (RandomLocation.TryGet(_locationName, out _current))
        {
            _blackboard.SetValue<Vector3>(_key, _current.transform.position);
            result = ExecutionStatus.Succeeded;
        }
        else result = ExecutionStatus.Failed;

        _blackboard = null;

        return result;
    }

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

    private void OnValidate() => name = ToString();

    public override string ToString() =>
        storeIn == null 
            ? "INVALID EXECUTABLE" 
            : $"Set {storeIn.name} as a random {location} location.";
}