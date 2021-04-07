using HiraEngine.Components.AI;
using HiraEngine.Components.Blackboard;

namespace UnityEngine.Internal
{
	public class PickProfessionExecutable : Executable
	{
		public PickProfessionExecutable Init(IBlackboardComponent blackboard, HiraBlackboardKey professionKey, DaySpenderProfession value)
		{
			_blackboard = blackboard;
			_professionKey = professionKey.Index;
            _value = value;
			return this;
		}

		private IBlackboardComponent _blackboard;
		private ushort _professionKey;
        private DaySpenderProfession _value;

		public override ExecutionStatus Execute(float deltaTime)
		{
			_blackboard.SetValue<DaySpenderProfession>(_professionKey, _value);
			_blackboard = null;
			return ExecutionStatus.Succeeded;
		}

        public override void Dispose() =>
            GenericPool<PickProfessionExecutable>.Return(this);
	}
	
	public class PickProfessionExecutableProvider : ScriptableObject, IExecutableProvider
	{
		[HiraCollectionDropdown(typeof(EnumKey))]
		[SerializeField] private HiraBlackboardKey professionKey = null;
        [SerializeField] private DaySpenderProfession profession = DaySpenderProfession.None;
        [SerializeField] private bool pickRandom = false;
		
		public Executable GetExecutable(HiraComponentContainer target, IBlackboardComponent blackboard) =>
            GenericPool<PickProfessionExecutable>.Retrieve().Init(blackboard, professionKey,
                pickRandom
                    ? (DaySpenderProfession) Random.Range(1, (int) DaySpenderProfession.Max)
                    : profession);

        private void OnValidate() => name = ToString();
        public override string ToString() => pickRandom ? "Pick a random profession" : $"Become a {profession}";
    }
}