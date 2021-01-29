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

		private bool _handleActive = false;
		private JobHandle _handle;
		private NativeArray<OpenCloseDoorTransitionData> _actions;
		private PlannerJob _job;
		private NativeArray<int> _plan;

		private void Awake()
		{
			
		}

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

			_handleActive = true;
			return _job.Schedule();

			// Debug.Log(job.Plan[0]);
			// Debug.Log((PlannerJobFunctionLibrary.Actions) job.Plan[1]);
			// Debug.Log((PlannerJobFunctionLibrary.Actions) job.Plan[2]);
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

			_planner = new Planner(ref blackboard, 2, PlannerJobFunctionLibrary.GoalOpenDoorTarget, actions, 1000, plan, PlannerCallback);
			
			_plannerActive = true;
			ThreadPool.QueueUserWorkItem(_planner.Execute);
		}

		private void PlannerCallback()
		{
			_plannerActive = false;
			Assert.AreEqual(2, _planner.Plan[0]);
			Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupKey, _planner.Plan[1]);
			Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.OpenDoor, _planner.Plan[2]);
		}

		private void Update()
		{
			if (!runEveryFrame) return;

			if (useJobs)
			{
				if (!_handleActive) _handle = SimpleTestJob();
			}
			else
			{
				if (!_plannerActive) SimpleTest();
			}
		}

		private void LateUpdate()
		{
			if (_handleActive)
			{
				_handleActive = false;
				_handle.Complete();
				Assert.AreEqual(2, _job.Plan[0]);
				Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.PickupKey, _job.Plan[1]);
				Assert.AreEqual((int) PlannerJobFunctionLibrary.Actions.OpenDoor, _job.Plan[2]);
				_job.Dispose();
				_actions.Dispose();
				_plan.Dispose();
			}
		}

		private void InvertedTest()
		{
		}

		private void InvertedTestRequiringStamina()
		{
		}
	}
}