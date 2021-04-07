using HiraEngine.Components.AI;
using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class PickRandomProfessionExecutable : Executable
	{
		public PickRandomProfessionExecutable Init(IBlackboardComponent blackboard, HiraBlackboardKey professionKey)
		{
			_blackboard = blackboard;
			_professionKey = professionKey.Index;
			return this;
		}

		private IBlackboardComponent _blackboard;
		private ushort _professionKey;

		public override ExecutionStatus Execute(float deltaTime)
		{
			var random = (DaySpenderProfession) Random.Range(1, (int) DaySpenderProfession.Max);
			_blackboard.SetValue<DaySpenderProfession>(_professionKey, random);
			_blackboard = null;
			return ExecutionStatus.Succeeded;
		}


		public override void Dispose()
		{
			GenericPool<PickRandomProfessionExecutable>.Return(this);
		}
	}
	
	public class PickRandomProfessionExecutableProvider : ScriptableObject, IExecutableProvider
	{
		[HiraCollectionDropdown(typeof(EnumKey))]
		[SerializeField] private HiraBlackboardKey professionKey = null;
		
		public Executable GetExecutable(HiraComponentContainer target, IBlackboardComponent blackboard) =>
			GenericPool<PickRandomProfessionExecutable>.Retrieve().Init(blackboard, professionKey);

        private void OnValidate() => name = ToString();
        public override string ToString() => "Pick random profession";
    }
}