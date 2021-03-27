using HiraEngine.Components.AI;
using HiraEngine.Components.Blackboard;
using UnityEngine;

public class PickRandomPatrolLocationExecutable : Executable, IPoolable
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
		var locations = Object.FindObjectsOfType<RandomLocation>(true);
		var count = locations.Length;
		if (count == 0)
		{
			_result = ExecutionStatus.Failed;
			return;
		}

		var random = locations[Random.Range(0, count)].Position;
		_blackboard.SetValue<Vector3>(_key, random);
		_result = ExecutionStatus.Succeeded;
	}

	public override ExecutionStatus Execute(float deltaTime) => _result;

	public void OnRetrieve()
	{
	}

	public void OnReturn()
	{
	}
}

public class PickRandomPatrolLocationExecutableProvider : ScriptableObject, IExecutableProvider
{
	[HiraCollectionDropdown(typeof(VectorKey))]
	[SerializeField] private HiraBlackboardKey storeIn = null;
	
	public Executable GetExecutable(GameObject target, IBlackboardComponent blackboard) =>
		GenericPool<PickRandomPatrolLocationExecutable>.Retrieve().Init(blackboard, storeIn);
}