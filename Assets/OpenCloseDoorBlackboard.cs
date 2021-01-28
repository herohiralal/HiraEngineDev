// ReSharper disable All

namespace UnityEngine
{
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct OpenCloseDoorBlackboard
    {
        public OpenCloseDoorBlackboard(
            bool? inDoorOpen = null,
            bool? inHasKey = null,
            bool? inHasCrowbar = null,
            bool? inHasStamina = null)
        {
            DoorOpen = inDoorOpen ?? false;
            HasKey = inHasKey ?? false;
            HasCrowbar = inHasCrowbar ?? false;
            HasStamina = inHasStamina ?? false;
        }

        public bool DoorOpen;
        public bool HasKey;
        public bool HasCrowbar;
        public bool HasStamina;

    }
    
    [System.Serializable]
    public class OpenCloseDoorBlackboardWrapper
    {
        public OpenCloseDoorBlackboardWrapper(
            bool? inDoorOpen = null,
            bool? inHasKey = null,
            bool? inHasCrowbar = null,
            bool? inHasStamina = null)
        {
            blackboard.DoorOpen = inDoorOpen ?? false;
            blackboard.HasKey = inHasKey ?? false;
            blackboard.HasCrowbar = inHasCrowbar ?? false;
            blackboard.HasStamina = inHasStamina ?? false;
        }

        ~OpenCloseDoorBlackboardWrapper()
        {
        }

        public event System.Action OnValueUpdate = delegate { };
        [SerializeField] private OpenCloseDoorBlackboard blackboard;
        
        public bool DoorOpen
        {
            get => blackboard.DoorOpen;
            set
            {
                blackboard.DoorOpen = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public bool HasKey
        {
            get => blackboard.HasKey;
            set
            {
                blackboard.HasKey = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public bool HasCrowbar
        {
            get => blackboard.HasCrowbar;
            set
            {
                blackboard.HasCrowbar = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public bool HasStamina
        {
            get => blackboard.HasStamina;
            set
            {
                blackboard.HasStamina = value;
                OnValueUpdate.Invoke();
            }
        }
    }
}
