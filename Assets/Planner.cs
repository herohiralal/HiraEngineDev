using System;
using Unity.Mathematics;

namespace UnityEngine
{
	public readonly struct Planner
	{
		private readonly OpenCloseDoorBlackboard[] _datasets;
		private readonly BlackboardQuery _goal;
		private readonly TransitionData[] _actions;
		private readonly int _actionsCount;
		private readonly float _maxFScore;
		public readonly int[] Plan;
		private readonly Action _callback;

		public Planner(ref OpenCloseDoorBlackboard dataset, int maxPlanLength, BlackboardQuery goal,
			TransitionData[] actions, float maxFScore, int[] plan, Action callback)
		{
			_datasets = new OpenCloseDoorBlackboard[maxPlanLength + 1];
			_datasets[0] = dataset;
			_goal = goal;
			_actionsCount = actions.Length;
			_actions = actions;
			_maxFScore = maxFScore;
			Plan = plan;
			_callback = callback;
		}

		public void Execute(object obj = null)
		{
			float threshold = GetHeuristic(0);

			while (true)
			{
				if (!PerformHeuristicEstimatedSearch(1, 0, threshold, out var score))
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

			MainThreadDispatcher.Schedule(_callback);
		}

		private unsafe bool PerformHeuristicEstimatedSearch(int index, float cost, float threshold, out float outScore)
		{

			fixed (OpenCloseDoorBlackboard* datasetsPtr = _datasets)
			{
				var heuristic = GetHeuristic(index - 1);
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

					if (action.Precondition.Invoke(datasetsPtr + index - 1) != 0) continue;

					_datasets[index] = _datasets[index - 1];
					action.Effect.Invoke(datasetsPtr + index);

					var scoreReceived = PerformHeuristicEstimatedSearch(index + 1, cost + action.Cost, threshold, out var score);

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
		}

		private unsafe int GetHeuristic(int index)
		{
			fixed(OpenCloseDoorBlackboard* datasetsPtr = _datasets)
				return _goal.Invoke(datasetsPtr + index);
		}
	}
}