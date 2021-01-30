// ReSharper disable Alla

namespace UnityEngine.Internal
{

	[Unity.Burst.BurstCompile]
	public readonly struct OpenCloseDoorTransitionData
	{
		[Unity.Collections.ReadOnly] public readonly int Identifier;
		[Unity.Collections.ReadOnly] public readonly int ArchetypeIndex;
		[Unity.Collections.ReadOnly] public readonly float Cost;

		public OpenCloseDoorTransitionData(int identifier, int archetypeIndex, float cost)
		{
			Identifier = identifier;
			ArchetypeIndex = archetypeIndex;
			Cost = cost;
		}
	}

	public static class OpenCloseDoorBlackboardArchetypeIndices
	{
		public const int GOAL_UNRECOGNIZED = 0;
		public const int GOAL_OPEN_DOOR = 1;
		public const int ACTION_UNRECOGNIZED = 0;
		public const int ACTION_OPEN_DOOR = 1;
		public const int ACTION_BREAK_DOOR = 2;
		public const int ACTION_BREAK_DOOR_WITHOUT_STAMINA = 3;
		public const int ACTION_PICKUP_KEY = 4;
		public const int ACTION_PICKUP_CROWBAR = 5;
		public const int ACTION_DRINK_WATER = 6;
	}
	
	[Unity.Burst.BurstCompile]
	public struct PlannerJob : Unity.Jobs.IJob
	{
		[Unity.Collections.DeallocateOnJobCompletion] private Unity.Collections.NativeArray<OpenCloseDoorBlackboard> _datasets;
		[Unity.Collections.ReadOnly] private readonly int _goal;
		[Unity.Collections.ReadOnly] private readonly Unity.Collections.NativeArray<OpenCloseDoorTransitionData> _actions;
		[Unity.Collections.ReadOnly] private readonly int _actionsCount;
		[Unity.Collections.ReadOnly] private readonly float _maxFScore;
		[Unity.Collections.WriteOnly] public Unity.Collections.NativeArray<int> Plan;

		public PlannerJob(ref OpenCloseDoorBlackboard dataset, int goal, int maxPlanLength,
			Unity.Collections.NativeArray<OpenCloseDoorTransitionData> actions, float maxFScore, Unity.Collections.NativeArray<int> plan)
		{
			_datasets = new Unity.Collections.NativeArray<OpenCloseDoorBlackboard>(maxPlanLength + 1, Unity.Collections.Allocator.TempJob) {[0] = dataset};
			_actionsCount = actions.Length;
			_actions = actions;
			_maxFScore = maxFScore;
			_goal = goal;
			Plan = plan;
		}

		public void Execute()
		{
			unsafe
			{
				var datasets = (OpenCloseDoorBlackboard*) Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr(_datasets);
				float threshold = HeuristicCheck(_goal, datasets);

				while (true)
				{
					if (!PerformHeuristicEstimatedSearch(datasets, 1, 0, threshold, out var score))
					{
						break;
					}

					if (score > _maxFScore)
					{
						Plan[0] = 0;
						break;
					}

					if (score < 0)
					{
						Debug.Log("Planning cancelled.");
						break;
					}

					threshold = score;
				}
			}
		}

		private unsafe bool PerformHeuristicEstimatedSearch(OpenCloseDoorBlackboard* datasets, int index, float cost, float threshold, out float outScore)
		{
			var heuristic = HeuristicCheck(_goal, datasets + index - 1);
			var fScore = cost + heuristic;
			if (fScore > threshold)
			{
				outScore = fScore;
				return true;
			}

			if (heuristic == 0)
			{
				Plan[0] = index - 1;
				outScore = -1;
				return false;
			}

			if (index == _datasets.Length)
			{
				outScore = float.MaxValue;
				return true;
			}

			var min = float.MaxValue;

			for (var i = 0; i < _actionsCount; i++)
			{
				var action = _actions[i];

				if (PreconditionCheck(action.ArchetypeIndex, datasets + index - 1) != 0) continue;
				// if (action.Precondition.Invoke(datasets + index - 1) != 0) continue;

				_datasets[index] = _datasets[index - 1];
				ApplyEffect(action.ArchetypeIndex, datasets + index);
				// action.Effect.Invoke(datasets + index);

				var scoreReceived = PerformHeuristicEstimatedSearch(datasets, index + 1, cost + action.Cost, threshold, out var score);
				
				_datasets[index] = _datasets[index - 1];
				
				if (!scoreReceived)
				{
					Plan[index] = action.Identifier;
					outScore = -1;
					return false;
				}

				_datasets[index] = _datasets[index - 1];
				min = Unity.Mathematics.math.min(score, min);
			}

			outScore = min;
			return true;
		}

		private static unsafe int HeuristicCheck(int target, OpenCloseDoorBlackboard* blackboard) =>
			target switch
			{
				OpenCloseDoorBlackboardArchetypeIndices.GOAL_UNRECOGNIZED => throw new System.Exception("Uninitialized goal data received by PlannerJob."),
				OpenCloseDoorBlackboardArchetypeIndices.GOAL_OPEN_DOOR => blackboard->DoorOpen ? 0 : 1,
				_ => throw new System.Exception($"Invalid goal data received by PlannerJob: {target}.")
			};

		private static unsafe int PreconditionCheck(int target, OpenCloseDoorBlackboard* blackboard) =>
			target switch
			{
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_UNRECOGNIZED => throw new System.Exception($"Uninitialized action data received by PlannerJob."),
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_OPEN_DOOR => (blackboard->HasKey ? 0 : 1) + (!blackboard->DoorOpen ? 0 : 1),
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_BREAK_DOOR => (blackboard->HasCrowbar ? 0 : 1) + (blackboard->HasStamina ? 0 : 1) + (!blackboard->DoorOpen ? 0 : 1),
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_BREAK_DOOR_WITHOUT_STAMINA => (blackboard->HasCrowbar ? 0 : 1) + (!blackboard->DoorOpen ? 0 : 1),
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_PICKUP_KEY => !blackboard->HasKey ? 0 : 1,
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_PICKUP_CROWBAR => !blackboard->HasCrowbar ? 0 : 1,
				OpenCloseDoorBlackboardArchetypeIndices.ACTION_DRINK_WATER => 0,
				_ => throw new System.Exception($"Invalid action data received by PlannerJob: {target}.")
			};

		private static unsafe void ApplyEffect(int target, OpenCloseDoorBlackboard* blackboard)
		{
			switch (target)
			{
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_UNRECOGNIZED:
					throw new System.Exception($"Uninitialized action data received by PlannerJob.");
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_OPEN_DOOR:
					blackboard->DoorOpen = true;
					break;
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_BREAK_DOOR:
					blackboard->DoorOpen = true;
					break;
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_BREAK_DOOR_WITHOUT_STAMINA:
					blackboard->DoorOpen = true;
					break;
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_PICKUP_KEY:
					blackboard->HasKey = true;
					break;
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_PICKUP_CROWBAR:
					blackboard->HasCrowbar = true;
					break;
				case OpenCloseDoorBlackboardArchetypeIndices.ACTION_DRINK_WATER:
					blackboard->HasStamina = true;
					break;
				default:
					throw new System.Exception($"Invalid action data received by PlannerJob: {target}.");
			}
		}
	}
}