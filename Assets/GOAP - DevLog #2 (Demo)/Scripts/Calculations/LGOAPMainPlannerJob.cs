using Unity.Collections;
using Unity.Jobs;

namespace LGOAPDemo
{
	public unsafe struct LGOAPMainPlannerJob<T> : IJob where T : unmanaged
	{
		public LGOAPMainPlannerJob
		(
			NativeArray<LGOAPGoalTargetData> goals,
			LGOAPGoalTargetData goal,
			NativeArray<LGOAPTaskData> actions,
			LGOAPPlannerResult parentResult,
			LGOAPPlannerResult previousResult,
			float maxFScore,
			NativeArray<T> datasets,
			LGOAPPlannerResult output
		)
		{
			_goals = goals;
			_goal = goal;
			_actions = actions;
			_parentResult = parentResult;
			_previousResult = previousResult;
			_maxFScore = maxFScore;
			_datasets = datasets;
			_output = output;
		}

		// domain data
		[ReadOnly] private readonly NativeArray<LGOAPGoalTargetData> _goals;
		private LGOAPGoalTargetData _goal;
		[ReadOnly] private NativeArray<LGOAPTaskData> _actions;

		// layering
		[ReadOnly] private readonly LGOAPPlannerResult _parentResult;

		// previous run
		[ReadOnly] private readonly LGOAPPlannerResult _previousResult;

		// settings
		[ReadOnly] private readonly float _maxFScore;

		// runtime data
		private NativeArray<T> _datasets;

		// output
		[WriteOnly] private LGOAPPlannerResult _output;

		public void Execute()
		{
			if (_parentResult.ResultType == LGOAPPlannerResultType.Failure)
			{
				_output.ResultType = LGOAPPlannerResultType.Failure;
				_output.Count = 0;
				return;
			}

			_goal = _goals[_parentResult[0]];

			if (_parentResult.ResultType == LGOAPPlannerResultType.Unchanged)
			{
			}
		}
	}
}