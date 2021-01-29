using System;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Assertions;

namespace UnityEngine
{
	public class OpenBreakDoorTest : MonoBehaviour
	{
		[SerializeField] private bool useJobs = true;
		[SerializeField] private bool runEveryFrame = false;

		private bool _plannerActive = false;
		private Planner _planner;

		private enum JobType
		{
			None, Simple, Inverted, InvertedRequiringStamina
		}

		private JobType _currentJob = JobType.None;
		private JobHandle _handle;
		private NativeArray<OpenCloseDoorTransitionData> _actions;
		private PlannerJob _job;
		private NativeArray<int> _plan;

		private JobHandle SimpleTestJob()
		{
			_actions = new NativeArray<OpenCloseDoorTransitionData>(4, Allocator.TempJob)
			{
				[0] = PlannerJobFunctionLibrary.GetOpenDoorAction(1),
				[1] = PlannerJobFunctionLibrary.GetPickupKeyAction(1),
				[2] = PlannerJobFunctionLibrary.GetPickupCrowbarAction(1),
				[3] = PlannerJobFunctionLibrary.GetBreakDoorWithoutStaminaAction(5)
			};

			_plan = new NativeArray<int>(3, Allocator.TempJob);

			var openCloseDoorBlackboard = new OpenCloseDoorBlackboard {DoorOpen = false, HasCrowbar = false, HasKey = false, HasStamina = false};
			_job = new PlannerJob(ref openCloseDoorBlackboard, 2, PlannerJobFunctionLibrary.GOAL_OPEN_DOOR_TARGET_DELEGATE,
				_actions, 1000, _plan);

			_currentJob = JobType.Simple;
			return _job.Schedule();
		}

		private unsafe void SimpleTest(object o = null)
		{
			var actions = new[]
			{
				PlannerJobFunctionLibrary.GetOpenDoorNonJobAction(1),
				PlannerJobFunctionLibrary.GetPickupKeyNonJobAction(1),
				PlannerJobFunctionLibrary.GetPickupCrowbarNonJobAction(1),
				PlannerJobFunctionLibrary.GetBreakDoorWithoutStaminaNonJobAction(5)
			};

			var plan = new int[3];

			var blackboard = new OpenCloseDoorBlackboard {DoorOpen = false, HasCrowbar = false, HasKey = false, HasStamina = false};

			_planner = new Planner(ref blackboard, 2, PlannerJobFunctionLibrary.GoalOpenDoorTarget, actions, 1000, plan, SimpleTestPlannerCallback);
			
			_plannerActive = true;
			ThreadPool.QueueUserWorkItem(_planner.Execute);
		}

		private void InvertedTestJob()
		{
		}

		private void InvertedTest()
		{
		}

		private JobHandle InvertedTestRequiringStaminaJob()
		{
			_actions = new NativeArray<OpenCloseDoorTransitionData>(5, Allocator.TempJob)
			{
				[0] = PlannerJobFunctionLibrary.GetOpenDoorAction(10),
				[1] = PlannerJobFunctionLibrary.GetPickupKeyAction(1),
				[2] = PlannerJobFunctionLibrary.GetPickupCrowbarAction(1),
				[3] = PlannerJobFunctionLibrary.GetBreakDoorAction(5),
				[4] = PlannerJobFunctionLibrary.GetDrinkWaterAction(1)
			};

			_plan = new NativeArray<int>(5, Allocator.TempJob);

			var openCloseDoorBlackboard = new OpenCloseDoorBlackboard {DoorOpen = false, HasCrowbar = false, HasKey = false, HasStamina = false};
			_job = new PlannerJob(ref openCloseDoorBlackboard, 5, PlannerJobFunctionLibrary.GOAL_OPEN_DOOR_TARGET_DELEGATE,
				_actions, 1000, _plan);

			_currentJob = JobType.InvertedRequiringStamina;
			return _job.Schedule();
		}

		private unsafe void InvertedTestRequiringStamina()
		{
			var actions = new[]
			{
				PlannerJobFunctionLibrary.GetOpenDoorNonJobAction(10),
				PlannerJobFunctionLibrary.GetPickupKeyNonJobAction(1),
				PlannerJobFunctionLibrary.GetPickupCrowbarNonJobAction(1),
				PlannerJobFunctionLibrary.GetBreakDoorNonJobAction(5),
				PlannerJobFunctionLibrary.GetDrinkWaterNonJobAction(1)
			};

			var plan = new int[5];

			var blackboard = new OpenCloseDoorBlackboard {DoorOpen = false, HasCrowbar = false, HasKey = false, HasStamina = false};

			_planner = new Planner(ref blackboard, 5, PlannerJobFunctionLibrary.GoalOpenDoorTarget,
				actions, 1000, plan, InvertedTestRequiringStaminaPlannerCallback);
			
			_plannerActive = true;
			ThreadPool.QueueUserWorkItem(_planner.Execute);
		}

		private void SimpleTestPlannerCallback()
		{
			_plannerActive = false;
			Assert.AreEqual(2, _planner.Plan[0]);
			Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupKey, _planner.Plan[1]);
			Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.OpenDoor, _planner.Plan[2]);
		}

		private void InvertedTestRequiringStaminaPlannerCallback()
		{
			_plannerActive = false;
			Assert.AreEqual(3, _planner.Plan[0]);
			try
			{
				Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupCrowbar, _planner.Plan[1]);
				Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.DrinkWater, _planner.Plan[2]);
			}
			catch (AssertionException)
			{
				Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.DrinkWater, _planner.Plan[1]);
				Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupCrowbar, _planner.Plan[2]);
			}
			Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.BreakDoor, _planner.Plan[3]);
		}

		private void Update()
		{
			
			if (_currentJob != JobType.None)
			{
				_handle.Complete();
				switch (_currentJob)
				{
					case JobType.Simple:
						Assert.AreEqual(2, _job.Plan[0]);
						Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupKey, _job.Plan[1]);
						Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.OpenDoor, _job.Plan[2]);
						break;
					case JobType.Inverted:
						break;
					case JobType.InvertedRequiringStamina:
						Assert.AreEqual(3, _job.Plan[0]);
						try
						{
							Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupCrowbar, _job.Plan[1]);
							Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.DrinkWater, _job.Plan[2]);
						}
						catch (AssertionException)
						{
							Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.DrinkWater, _job.Plan[1]);
							Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupCrowbar, _job.Plan[2]);
						}

						Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.BreakDoor, _job.Plan[3]);
						break;
				}

				_currentJob = JobType.None;
				_job.Dispose();
				_actions.Dispose();
				_plan.Dispose();
			}
			
			if (!runEveryFrame) return;

			if (useJobs)
			{
				if (_currentJob == JobType.None) _handle = InvertedTestRequiringStaminaJob();
			}
			else
			{
				if (!_plannerActive) InvertedTestRequiringStamina();
			}
		}
	}
}