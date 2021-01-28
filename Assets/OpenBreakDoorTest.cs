using System;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine
{
    public class OpenBreakDoorTest : MonoBehaviour
    {
        private enum Actions
        {
            OpenDoor, BreakDoor, PickupKey, PickupCrowbar, DrinkWater
        }

        private void Awake()
        {
            
        }

        private void SimpleTest()
        {
            using var actions = new NativeArray<OpenCloseDoorTransitionData>(4, Allocator.TempJob)
            {
                [0] = GetOpenDoorAction(1),
                [1] = GetPickupKeyAction(1),
                [2] = GetPickupCrowbarAction(1),
                [3] = GetBreakDoorAction(5)
            };

            using var plan = new NativeArray<int>(2, Allocator.TempJob);
            
            var openCloseDoorBlackboard = new OpenCloseDoorBlackboard {DoorOpen = false, HasCrowbar = false, HasKey = false, HasStamina = false};
            using var job = new PlannerJob(ref openCloseDoorBlackboard, 2, PlannerJobFunctionLibrary.GoalOpenDoorTargetDelegate,
                actions, 1000, plan);
            
            var jobHandle = job.Schedule();
            jobHandle.Complete();
            
            Debug.Log((Actions) job.Plan[0]);
            Debug.Log((Actions) job.Plan[1]);
        }

        private void InvertedTest()
        {
        }

        private void InvertedTestRequiringStamina()
        {
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Simple Test"))
                SimpleTest();
        }

        private static OpenCloseDoorTransitionData GetOpenDoorAction(float cost)
        {
            return new OpenCloseDoorTransitionData
            {
                Cost = cost,
                Effect = PlannerJobFunctionLibrary.ActionOpenDoorEffectDelegate,
                OuterIndex = (int) Actions.OpenDoor,
                Precondition = PlannerJobFunctionLibrary.ActionOpenDoorPreConditionDelegate
            };
        }

        private static OpenCloseDoorTransitionData GetBreakDoorAction(float cost)
        {
            return new OpenCloseDoorTransitionData
            {
                Cost = cost,
                Effect = PlannerJobFunctionLibrary.ActionBreakDoorEffectDelegate,
                OuterIndex = (int) Actions.BreakDoor,
                Precondition = PlannerJobFunctionLibrary.ActionBreakDoorPreConditionDelegate
            };
        }

        private static OpenCloseDoorTransitionData GetPickupKeyAction(float cost)
        {
            return new OpenCloseDoorTransitionData
            {
                Cost = cost,
                Effect = PlannerJobFunctionLibrary.ActionGetKeyEffectDelegate,
                OuterIndex = (int) Actions.PickupKey,
                Precondition = PlannerJobFunctionLibrary.ActionGetKeyPreConditionDelegate
            };
        }

        private static OpenCloseDoorTransitionData GetPickupCrowbarAction(float cost)
        {
            return new OpenCloseDoorTransitionData
            {
                Cost = cost,
                Effect = PlannerJobFunctionLibrary.ActionGetCrowbarEffectDelegate,
                OuterIndex = (int) Actions.PickupCrowbar,
                Precondition = PlannerJobFunctionLibrary.ActionGetCrowbarPreConditionDelegate
            };
        }

        private static OpenCloseDoorTransitionData GetDrinkWaterAction(float cost)
        {
            return new OpenCloseDoorTransitionData
            {
                Cost = cost,
                Effect = PlannerJobFunctionLibrary.ActionDrinkWaterEffectDelegate,
                OuterIndex = (int) Actions.DrinkWater,
                Precondition = PlannerJobFunctionLibrary.ActionDrinkWaterPreConditionDelegate
            };
        }
    }

    [BurstCompile]
    public struct OpenCloseDoorTransitionData
    {
        [ReadOnly] public int OuterIndex;
        [ReadOnly] public FunctionPointer<BlackboardQuery> Precondition;
        [ReadOnly] public FunctionPointer<BlackboardModification> Effect;
        [ReadOnly] public float Cost;
    }

    public unsafe delegate int BlackboardQuery(OpenCloseDoorBlackboard* blackboard);

    public unsafe delegate void BlackboardModification(OpenCloseDoorBlackboard* blackboard);

    [BurstCompile]
    public static class PlannerJobFunctionLibrary
    {
        // goal

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int GoalOpenDoorTarget(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->DoorOpen ? 0 : 1;

        // get key

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int ActionGetKeyPreCondition(OpenCloseDoorBlackboard* blackboard) =>
            !blackboard->HasKey ? 0 : 1;

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardModification))]
        public static unsafe void ActionGetKeyEffect(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->HasKey = true;

        // get crowbar

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int ActionGetCrowbarPreCondition(OpenCloseDoorBlackboard* blackboard) =>
            !blackboard->HasCrowbar ? 0 : 1;

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardModification))]
        public static unsafe void ActionGetCrowbarEffect(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->HasCrowbar = true;

        // drink water

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int ActionDrinkWaterPreCondition(OpenCloseDoorBlackboard* blackboard) =>
            0;

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardModification))]
        public static unsafe void ActionDrinkWaterEffect(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->HasStamina = true;

        // open door

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int ActionOpenDoorPreCondition(OpenCloseDoorBlackboard* blackboard) =>
            (blackboard->HasKey ? 0 : 1) + (!blackboard->DoorOpen ? 0 : 1);

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardModification))]
        public static unsafe void ActionOpenDoorEffect(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->DoorOpen = true;

        // break door

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int ActionBreakDoorPreCondition(OpenCloseDoorBlackboard* blackboard) =>
            (blackboard->HasCrowbar ? 0 : 1) + (blackboard->HasStamina ? 0 : 1) + (!blackboard->DoorOpen ? 0 : 1);

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardModification))]
        public static unsafe void ActionBreakDoorEffect(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->DoorOpen = true;

        // break door without stamina

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardQuery))]
        public static unsafe int ActionBreakDoorWithoutStaminaPreCondition(OpenCloseDoorBlackboard* blackboard) =>
            (blackboard->HasCrowbar ? 0 : 1) + (!blackboard->DoorOpen ? 0 : 1);

        [BurstCompile]
        [MonoPInvokeCallback(typeof(BlackboardModification))]
        public static unsafe void ActionBreakDoorWithoutStaminaEffect(OpenCloseDoorBlackboard* blackboard) =>
            blackboard->DoorOpen = true;
        
        // function pointers

        public static unsafe FunctionPointer<BlackboardQuery> GoalOpenDoorTargetDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.GoalOpenDoorTarget);

        public static unsafe FunctionPointer<BlackboardQuery> ActionGetKeyPreConditionDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.ActionGetKeyPreCondition);

        public static unsafe FunctionPointer<BlackboardQuery> ActionGetCrowbarPreConditionDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.ActionGetCrowbarPreCondition);

        public static unsafe FunctionPointer<BlackboardQuery> ActionDrinkWaterPreConditionDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.ActionDrinkWaterPreCondition);

        public static unsafe FunctionPointer<BlackboardQuery> ActionOpenDoorPreConditionDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.ActionOpenDoorPreCondition);

        public static unsafe FunctionPointer<BlackboardQuery> ActionBreakDoorPreConditionDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.ActionBreakDoorPreCondition);

        public static unsafe FunctionPointer<BlackboardQuery> ActionBreakDoorWithoutStaminaPreConditionDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardQuery>(PlannerJobFunctionLibrary.ActionBreakDoorWithoutStaminaPreCondition);

        public static unsafe FunctionPointer<BlackboardModification> ActionGetKeyEffectDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardModification>(PlannerJobFunctionLibrary.ActionGetKeyEffect);

        public static unsafe FunctionPointer<BlackboardModification> ActionGetCrowbarEffectDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardModification>(PlannerJobFunctionLibrary.ActionGetCrowbarEffect);

        public static unsafe FunctionPointer<BlackboardModification> ActionDrinkWaterEffectDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardModification>(PlannerJobFunctionLibrary.ActionDrinkWaterEffect);

        public static unsafe FunctionPointer<BlackboardModification> ActionOpenDoorEffectDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardModification>(PlannerJobFunctionLibrary.ActionOpenDoorEffect);

        public static unsafe FunctionPointer<BlackboardModification> ActionBreakDoorEffectDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardModification>(PlannerJobFunctionLibrary.ActionBreakDoorEffect);

        public static unsafe FunctionPointer<BlackboardModification> ActionBreakDoorWithoutStaminaEffectDelegate =
            BurstCompiler.CompileFunctionPointer<BlackboardModification>(PlannerJobFunctionLibrary.ActionBreakDoorWithoutStaminaEffect);
    }

    [BurstCompile]
    public struct PlannerJob : IJob, IDisposable
    {
        [ReadOnly] private OpenCloseDoorBlackboard _dataset;
        [ReadOnly] private NativeArray<OpenCloseDoorBlackboard> _datasets;
        [ReadOnly] private readonly FunctionPointer<BlackboardQuery> _goal;
        [ReadOnly] private readonly NativeArray<OpenCloseDoorTransitionData> _actions;
        [ReadOnly] private readonly float _maxFScore;
        [ReadOnly] private readonly int _maxPlanLength;
        public NativeArray<int> Plan;

        public PlannerJob(ref OpenCloseDoorBlackboard dataset, int maxPlanLength, FunctionPointer<BlackboardQuery> goal,
            NativeArray<OpenCloseDoorTransitionData> actions, float maxFScore, NativeArray<int> plan)
        {
            _dataset = dataset;
            _maxPlanLength = maxPlanLength;
            _datasets = new NativeArray<OpenCloseDoorBlackboard>(maxPlanLength + 1, Allocator.TempJob);
            _datasets[0] = _dataset;
            _goal = goal;
            _actions = actions;
            _maxFScore = maxFScore;
            Plan = plan;
        }

        public void Execute()
        {
            float threshold = GetHeuristic(0);
        }

        private unsafe int GetHeuristic(int index)
        {
            var v = _datasets[index];
            return _goal.Invoke(&v);
        }

        public void Dispose()
        {
            _datasets.Dispose();
        }
    }
}