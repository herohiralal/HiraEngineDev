using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine
{
	[BurstCompile]
	public struct PlannerJob : IJob, IDisposable
	{
		private NativeArray<OpenCloseDoorBlackboard> _datasets;
		[ReadOnly] private readonly FunctionPointer<BlackboardQuery> _goal;
		[ReadOnly] private readonly NativeArray<OpenCloseDoorTransitionData> _actions;
		[ReadOnly] private readonly int _actionsCount;
		[ReadOnly] private readonly float _maxFScore;
		public NativeArray<int> Plan;

		public PlannerJob(ref OpenCloseDoorBlackboard dataset, int maxPlanLength, FunctionPointer<BlackboardQuery> goal,
			NativeArray<OpenCloseDoorTransitionData> actions, float maxFScore, NativeArray<int> plan)
		{
			_datasets = new NativeArray<OpenCloseDoorBlackboard>(maxPlanLength + 1, Allocator.TempJob);
			_datasets[0] = dataset;
			_goal = goal;
			_actionsCount = actions.Length;
			_actions = actions;
			_maxFScore = maxFScore;
			Plan = plan;
		}

		public void Execute()
		{
			unsafe
			{
				var datasets = (OpenCloseDoorBlackboard*) _datasets.GetUnsafePtr();
				float threshold = _goal.Invoke(datasets);

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
			var heuristic = _goal.Invoke(datasets + index - 1);
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
				
				if (action.Precondition.Invoke(datasets + index - 1) != 0) continue;

				_datasets[index] = _datasets[index - 1];
				action.Effect.Invoke(datasets + index);

				var scoreReceived = PerformHeuristicEstimatedSearch(datasets, index + 1, cost + action.Cost, threshold, out var score);
				
				_datasets[index] = _datasets[index - 1];
				
				if (!scoreReceived)
				{
					Plan[index] = action.OuterIndex;
					outScore = -1;
					return false;
				}

				_datasets[index] = _datasets[index - 1];
				min = math.min(score, min);
			}

			outScore = min;
			return true;
		}

		public void Dispose() =>
			_datasets.Dispose();
	}
}