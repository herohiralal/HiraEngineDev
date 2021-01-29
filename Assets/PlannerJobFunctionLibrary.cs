using AOT;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.Profiling;

namespace UnityEngine
{

	public unsafe delegate int BlackboardQuery(OpenCloseDoorBlackboard* blackboard);

	public unsafe delegate void BlackboardModification(OpenCloseDoorBlackboard* blackboard);

	[BurstCompile]
	public struct OpenCloseDoorTransitionData
	{
		[ReadOnly] public int OuterIndex;
		[ReadOnly] public PlannerJobFunctionLibrary.Actions Type;
		//[ReadOnly] public FunctionPointer<BlackboardQuery> Precondition;
		//[ReadOnly] public FunctionPointer<BlackboardModification> Effect;
		[ReadOnly] public float Cost;
	}
	
	public readonly struct TransitionData
	{
		public readonly int OuterIndex;
		public readonly BlackboardQuery Precondition;
		public readonly BlackboardModification Effect;
		public readonly float Cost;

		public TransitionData(int outerIndex, BlackboardQuery precondition, BlackboardModification effect, float cost)
		{
			OuterIndex = outerIndex;
			Precondition = precondition;
			Effect = effect;
			Cost = cost;
		}
	}
	
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

		public static readonly unsafe FunctionPointer<BlackboardQuery> GOAL_OPEN_DOOR_TARGET_DELEGATE =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(GoalOpenDoorTarget);

		private static readonly unsafe FunctionPointer<BlackboardQuery> action_get_key_pre_condition_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(ActionGetKeyPreCondition);

		private static readonly unsafe FunctionPointer<BlackboardQuery> action_get_crowbar_pre_condition_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(ActionGetCrowbarPreCondition);

		private static readonly unsafe FunctionPointer<BlackboardQuery> action_drink_water_pre_condition_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(ActionDrinkWaterPreCondition);

		private static readonly unsafe FunctionPointer<BlackboardQuery> action_open_door_pre_condition_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(ActionOpenDoorPreCondition);

		private static readonly unsafe FunctionPointer<BlackboardQuery> action_break_door_pre_condition_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(ActionBreakDoorPreCondition);

		private static readonly unsafe FunctionPointer<BlackboardQuery> action_break_door_without_stamina_pre_condition_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardQuery>(ActionBreakDoorWithoutStaminaPreCondition);

		private static readonly unsafe FunctionPointer<BlackboardModification> action_get_key_effect_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardModification>(ActionGetKeyEffect);

		private static readonly unsafe FunctionPointer<BlackboardModification> action_get_crowbar_effect_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardModification>(ActionGetCrowbarEffect);

		private static readonly unsafe FunctionPointer<BlackboardModification> action_drink_water_effect_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardModification>(ActionDrinkWaterEffect);

		private static readonly unsafe FunctionPointer<BlackboardModification> action_open_door_effect_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardModification>(ActionOpenDoorEffect);

		private static readonly unsafe FunctionPointer<BlackboardModification> action_break_door_effect_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardModification>(ActionBreakDoorEffect);

		private static readonly unsafe FunctionPointer<BlackboardModification> action_break_door_without_stamina_effect_delegate =
			BurstCompiler.CompileFunctionPointer<BlackboardModification>(ActionBreakDoorWithoutStaminaEffect);
		
		public enum Actions
		{
			Unrecognized = 0,
			OpenDoor = 1,
			BreakDoor = 2,
			BreakDoorWithoutStamina = 3,
			PickupKey = 4,
			PickupCrowbar = 5,
			DrinkWater = 6
		}

		public static OpenCloseDoorTransitionData GetOpenDoorAction(float cost)
		{
			return new OpenCloseDoorTransitionData
			{
				Cost = cost,
				//Effect = action_open_door_effect_delegate,
				OuterIndex = (int) Actions.OpenDoor,
				Type = Actions.OpenDoor,
				//Precondition = action_open_door_pre_condition_delegate
			};
		}

		public static OpenCloseDoorTransitionData GetBreakDoorAction(float cost)
		{
			return new OpenCloseDoorTransitionData
			{
				Cost = cost,
				//Effect = action_break_door_effect_delegate,
				OuterIndex = (int) Actions.BreakDoor,
				Type = Actions.BreakDoor,
				//Precondition = action_break_door_pre_condition_delegate
			};
		}

		public static OpenCloseDoorTransitionData GetBreakDoorWithoutStaminaAction(float cost)
		{
			return new OpenCloseDoorTransitionData
			{
				Cost = cost,
				//Effect = action_break_door_without_stamina_effect_delegate,
				OuterIndex = (int) Actions.BreakDoorWithoutStamina,
				Type = Actions.BreakDoorWithoutStamina,
				//Precondition = action_break_door_without_stamina_pre_condition_delegate
			};
		}

		public static OpenCloseDoorTransitionData GetPickupKeyAction(float cost)
		{
			return new OpenCloseDoorTransitionData
			{
				Cost = cost,
				//Effect = action_get_key_effect_delegate,
				OuterIndex = (int) Actions.PickupKey,
				Type = Actions.PickupKey,
				//Precondition = action_get_key_pre_condition_delegate
			};
		}

		public static OpenCloseDoorTransitionData GetPickupCrowbarAction(float cost)
		{
			return new OpenCloseDoorTransitionData
			{
				Cost = cost,
				//Effect = action_get_crowbar_effect_delegate,
				OuterIndex = (int) Actions.PickupCrowbar,
				Type = Actions.PickupCrowbar,
				//Precondition = action_get_crowbar_pre_condition_delegate
			};
		}

		public static OpenCloseDoorTransitionData GetDrinkWaterAction(float cost)
		{
			return new OpenCloseDoorTransitionData
			{
				Cost = cost,
				//Effect = action_drink_water_effect_delegate,
				OuterIndex = (int) Actions.DrinkWater,
				Type = Actions.DrinkWater,
				//Precondition = action_drink_water_pre_condition_delegate
			};
		}

		public static unsafe TransitionData GetOpenDoorNonJobAction(float cost) =>
			new TransitionData((int) Actions.OpenDoor, ActionOpenDoorPreCondition, ActionOpenDoorEffect, cost);

		public static unsafe TransitionData GetBreakDoorNonJobAction(float cost) =>
			new TransitionData((int) Actions.BreakDoor, ActionBreakDoorPreCondition, ActionBreakDoorEffect, cost);

		public static unsafe TransitionData GetBreakDoorWithoutStaminaNonJobAction(float cost) =>
			new TransitionData((int) Actions.BreakDoor, ActionBreakDoorWithoutStaminaPreCondition, ActionBreakDoorWithoutStaminaEffect, cost);

		public static unsafe TransitionData GetPickupKeyNonJobAction(float cost) =>
			new TransitionData((int) Actions.PickupKey, ActionGetKeyPreCondition, ActionGetKeyEffect, cost);

		public static unsafe TransitionData GetPickupCrowbarNonJobAction(float cost) =>
			new TransitionData((int) Actions.PickupCrowbar, ActionGetCrowbarPreCondition, ActionGetCrowbarEffect, cost);

		public static unsafe TransitionData GetDrinkWaterNonJobAction(float cost) =>
			new TransitionData((int) Actions.DrinkWater, ActionDrinkWaterPreCondition, ActionDrinkWaterEffect, cost);
	}
}