// ReSharper disable All

namespace UnityEngine
{
    [System.Serializable]
    [Unity.Burst.BurstCompile]
    public struct TreeWatererBlackboard : UnityEngine.IBlackboard
    {
        public TreeWatererBlackboard(
            float? inInsecurity = null,
            int? inWaterCarriedByAgent = null,
            int? inWaterInPool = null,
            bool? inWaterCarriedByPlant = null,
            bool? inWesternPatrolPointActive = null,
            bool? inEasternPatrolPointActive = null,
            bool? inDoorOpen = null,
            bool? inHasKey = null,
            bool? inHasCrowbar = null)
        {
            Insecurity = inInsecurity ?? 100f;
            WaterCarriedByAgent = inWaterCarriedByAgent ?? 0;
            WaterInPool = inWaterInPool ?? 200;
            WaterCarriedByPlant = inWaterCarriedByPlant ?? false;
            WesternPatrolPointActive = inWesternPatrolPointActive ?? true;
            EasternPatrolPointActive = inEasternPatrolPointActive ?? true;
            DoorOpen = inDoorOpen ?? false;
            HasKey = inHasKey ?? false;
            HasCrowbar = inHasCrowbar ?? false;
        }

        public float Insecurity;
        public int WaterCarriedByAgent;
        public int WaterInPool;
        public bool WaterCarriedByPlant;
        public bool WesternPatrolPointActive;
        public bool EasternPatrolPointActive;
        public bool DoorOpen;
        public bool HasKey;
        public bool HasCrowbar;
        
        [Unity.Burst.BurstCompile]
        public static class ArchetypeIndices
        {
            public const int GOAL_UNINITIALIZED = 0;
            public const int GOAL_WATER_PLANT = 1;
            public const int GOAL_PATROL = 2;
            public const int GOAL_COUNT = 2;
            public const int ACTION_UNINITIALIZED = 0;
            public const int ACTION_GET_KEY = 1;
            public const int ACTION_GET_CROWBAR = 2;
            public const int ACTION_OPEN_DOOR = 3;
            public const int ACTION_BREAK_DOOR = 4;
            public const int ACTION_PATROL_WESTERN_PATROL_POINT = 5;
            public const int ACTION_PATROL_EASTERN_PATROL_POINT = 6;
            public const int ACTION_WATER_TREE = 7;
            public const int ACTION_GET_WATER = 8;
            public const int ACTION_COUNT = 8;
        }
        
        [Unity.Burst.BurstCompile]
        public bool GetGoalValidity(int target) =>
            target switch
            {
                ArchetypeIndices.GOAL_UNINITIALIZED => throw new System.Exception("Uninitialized goal data received in TreeWatererBlackboard."),
                ArchetypeIndices.GOAL_WATER_PLANT => true && !(WaterCarriedByPlant),
                ArchetypeIndices.GOAL_PATROL => true && !(Insecurity < 10f),
                _ => throw new System.Exception($"Invalid goal data received by TreeWatererBlackboard: {target}.")
            };
        
        [Unity.Burst.BurstCompile]
        public int GetGoalHeuristic(int target) =>
            target switch
            {
                ArchetypeIndices.GOAL_UNINITIALIZED => throw new System.Exception("Uninitialized goal data received by TreeWatererBlackboard."),
                ArchetypeIndices.GOAL_WATER_PLANT => ((WaterCarriedByPlant) ? 0 : 1),
                ArchetypeIndices.GOAL_PATROL => ((Insecurity < 10f) ? 0 : 1),
                _ => throw new System.Exception($"Invalid goal data received by TreeWatererBlackboard: {target}.")
            };
        
        [Unity.Burst.BurstCompile]
        public bool GetActionApplicability(int target) =>
            target switch
            {
                ArchetypeIndices.ACTION_UNINITIALIZED => throw new System.Exception("Uninitialized action data received in TreeWatererBlackboard."),
                ArchetypeIndices.ACTION_GET_KEY => (!HasKey),
                ArchetypeIndices.ACTION_GET_CROWBAR => (!HasCrowbar),
                ArchetypeIndices.ACTION_OPEN_DOOR => (HasKey) && (!DoorOpen),
                ArchetypeIndices.ACTION_BREAK_DOOR => (HasCrowbar) && (!DoorOpen),
                ArchetypeIndices.ACTION_PATROL_WESTERN_PATROL_POINT => (WesternPatrolPointActive) && (DoorOpen),
                ArchetypeIndices.ACTION_PATROL_EASTERN_PATROL_POINT => (EasternPatrolPointActive) && (DoorOpen),
                ArchetypeIndices.ACTION_WATER_TREE => (DoorOpen) && (WaterCarriedByAgent >= 1) && (!WaterCarriedByPlant),
                ArchetypeIndices.ACTION_GET_WATER => (WaterInPool >= 2) && (DoorOpen),
                _ => throw new System.Exception($"Invalid action data received by TreeWatererBlackboard: {target}.")
            };
        
        [Unity.Burst.BurstCompile]
        public void ApplyActionEffect(int target)
        {
            switch (target)
            {
                case ArchetypeIndices.ACTION_UNINITIALIZED: throw new System.Exception("Uninitialized action data received in TreeWatererBlackboard.");
                case ArchetypeIndices.ACTION_GET_KEY: HasKey = true; break;
                case ArchetypeIndices.ACTION_GET_CROWBAR: HasCrowbar = true; break;
                case ArchetypeIndices.ACTION_OPEN_DOOR: HasKey = false;  DoorOpen = true; break;
                case ArchetypeIndices.ACTION_BREAK_DOOR: HasCrowbar = false;  DoorOpen = true; break;
                case ArchetypeIndices.ACTION_PATROL_WESTERN_PATROL_POINT: WesternPatrolPointActive = false;  Insecurity -= 50f; break;
                case ArchetypeIndices.ACTION_PATROL_EASTERN_PATROL_POINT: EasternPatrolPointActive = false;  Insecurity -= 50f; break;
                case ArchetypeIndices.ACTION_WATER_TREE: WaterCarriedByAgent -= 1;  WaterCarriedByPlant = true; break;
                case ArchetypeIndices.ACTION_GET_WATER: WaterCarriedByAgent += 2;  WaterInPool -= 2; break;
                default: throw new System.Exception($"Invalid action data received by TreeWatererBlackboard: {target}.");
            }
        }
    }
    
    [System.Serializable]
    public class TreeWatererBlackboardWrapper : UnityEngine.IBlackboard
    {
        public TreeWatererBlackboardWrapper(
            float? inInsecurity = null,
            int? inWaterCarriedByAgent = null,
            int? inWaterInPool = null,
            bool? inWaterCarriedByPlant = null,
            bool? inWesternPatrolPointActive = null,
            bool? inEasternPatrolPointActive = null,
            bool? inDoorOpen = null,
            bool? inHasKey = null,
            bool? inHasCrowbar = null)
        {
            blackboard.Insecurity = inInsecurity ?? 100f;
            blackboard.WaterCarriedByAgent = inWaterCarriedByAgent ?? 0;
            blackboard.WaterInPool = inWaterInPool ?? 200;
            blackboard.WaterCarriedByPlant = inWaterCarriedByPlant ?? false;
            blackboard.WesternPatrolPointActive = inWesternPatrolPointActive ?? true;
            blackboard.EasternPatrolPointActive = inEasternPatrolPointActive ?? true;
            blackboard.DoorOpen = inDoorOpen ?? false;
            blackboard.HasKey = inHasKey ?? false;
            blackboard.HasCrowbar = inHasCrowbar ?? false;
        }

        ~TreeWatererBlackboardWrapper()
        {
        }

        public event System.Action OnValueUpdate = delegate { };
        [SerializeField] public TreeWatererBlackboard blackboard;
        
        public float Insecurity
        {
            get => blackboard.Insecurity;
            set
            {
                blackboard.Insecurity = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public int WaterCarriedByAgent
        {
            get => blackboard.WaterCarriedByAgent;
            set
            {
                blackboard.WaterCarriedByAgent = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public int WaterInPool
        {
            get => blackboard.WaterInPool;
            set
            {
                blackboard.WaterInPool = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public bool WaterCarriedByPlant
        {
            get => blackboard.WaterCarriedByPlant;
            set
            {
                blackboard.WaterCarriedByPlant = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public bool WesternPatrolPointActive
        {
            get => blackboard.WesternPatrolPointActive;
            set
            {
                blackboard.WesternPatrolPointActive = value;
                OnValueUpdate.Invoke();
            }
        }
        
        public bool EasternPatrolPointActive
        {
            get => blackboard.EasternPatrolPointActive;
            set
            {
                blackboard.EasternPatrolPointActive = value;
                OnValueUpdate.Invoke();
            }
        }
        
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
        
        public bool GetBooleanValue(string keyName)
        {
            switch (keyName)
            {
                case "WaterCarriedByPlant":
                    return WaterCarriedByPlant;
                case "WesternPatrolPointActive":
                    return WesternPatrolPointActive;
                case "EasternPatrolPointActive":
                    return EasternPatrolPointActive;
                case "DoorOpen":
                    return DoorOpen;
                case "HasKey":
                    return HasKey;
                case "HasCrowbar":
                    return HasCrowbar;
                default:
                    Debug.LogError($"Key not recognized: {keyName}. Blackboard: TreeWatererBlackboard.");
                    return default;
            }
        }
        
        public void SetBooleanValue(string keyName, bool newValue)
        {
            switch (keyName)
            {
                case "WaterCarriedByPlant":
                    WaterCarriedByPlant = newValue;
                    return;
                case "WesternPatrolPointActive":
                    WesternPatrolPointActive = newValue;
                    return;
                case "EasternPatrolPointActive":
                    EasternPatrolPointActive = newValue;
                    return;
                case "DoorOpen":
                    DoorOpen = newValue;
                    return;
                case "HasKey":
                    HasKey = newValue;
                    return;
                case "HasCrowbar":
                    HasCrowbar = newValue;
                    return;
                default:
                    Debug.LogError($"Key not recognized: {keyName}. Blackboard: TreeWatererBlackboard.");
                    return;
            }
        }
        
        public int GetIntegerValue(string keyName)
        {
            switch (keyName)
            {
                case "WaterCarriedByAgent":
                    return WaterCarriedByAgent;
                case "WaterInPool":
                    return WaterInPool;
                default:
                    Debug.LogError($"Key not recognized: {keyName}. Blackboard: TreeWatererBlackboard.");
                    return default;
            }
        }
        
        public void SetIntegerValue(string keyName, int newValue)
        {
            switch (keyName)
            {
                case "WaterCarriedByAgent":
                    WaterCarriedByAgent = newValue;
                    return;
                case "WaterInPool":
                    WaterInPool = newValue;
                    return;
                default:
                    Debug.LogError($"Key not recognized: {keyName}. Blackboard: TreeWatererBlackboard.");
                    return;
            }
        }
        
        public float GetFloatValue(string keyName)
        {
            switch (keyName)
            {
                case "Insecurity":
                    return Insecurity;
                default:
                    Debug.LogError($"Key not recognized: {keyName}. Blackboard: TreeWatererBlackboard.");
                    return default;
            }
        }
        
        public void SetFloatValue(string keyName, float newValue)
        {
            switch (keyName)
            {
                case "Insecurity":
                    Insecurity = newValue;
                    return;
                default:
                    Debug.LogError($"Key not recognized: {keyName}. Blackboard: TreeWatererBlackboard.");
                    return;
            }
        }
        
        public bool GetGoalValidity(int target) => blackboard.GetGoalValidity(target);
        
        public int GetGoalHeuristic(int target) => blackboard.GetGoalHeuristic(target);
        
        public bool GetActionApplicability(int target) => blackboard.GetActionApplicability(target);
        
        public void ApplyActionEffect(int target)
        {
            blackboard.ApplyActionEffect(target);
            OnValueUpdate.Invoke();
        }
        
        public TGoal GetGoal<TGoal>(TGoal[] goals) where TGoal : UnityEngine.IActualGoal => UnityEngine.GoalCalculator.GetGoal(ref blackboard, goals);
        
        public TGoal GetGoal<TGoal>(System.Collections.Generic.List<TGoal> goals) where TGoal : UnityEngine.IActualGoal => UnityEngine.GoalCalculator.GetGoal(ref blackboard, goals);
    }
}
