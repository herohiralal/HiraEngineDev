﻿using System.Collections;
using System.Collections.Generic;
using HiraEngine.Components.AI.LGOAP;
using UnityEngine.SceneManagement;

namespace UnityEngine.Internal
{
    public class Initializer : MonoBehaviour
	{
		[SerializeField] private HiraBlackboardTemplate template = null;
		[SerializeField] private GoalOrientedActionPlannerDomain domain = null;

		[SerializeField] private GameObject agentPrefab = null;
		private readonly Stack<IInitializable> _initializedObjects = new Stack<IInitializable>();

		[SerializeField] private StringReference sceneName = null;

		[SerializeField] private GameObject mainMenu = null;
		[SerializeField] private GameObject loader = null;
		[SerializeField] private GameObject gameScreen = null;

		private void Awake()
		{
			loader.SetActive(false);
			gameScreen.SetActive(false);
			mainMenu.SetActive(true);
			
            template.Initialize();

			domain.Initialize();
		}

		public void Begin() => StartCoroutine(BeginCoroutine());

		private IEnumerator BeginCoroutine()
		{
			loader.SetActive(true);
			mainMenu.SetActive(false);
			
			var sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			while (!sceneLoadOperation.isDone) yield return null;

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

			var agent = Instantiate(agentPrefab, transform.position, Quaternion.identity, null);

			var blackboard = agent.GetComponent<HiraBlackboard>();
			if (blackboard != null && blackboard is IInitializable initializableBlackboard)
			{
				initializableBlackboard.Initialize();
				_initializedObjects.Push(initializableBlackboard);
				while (initializableBlackboard.InitializationStatus != InitializationState.Active)
					yield return null;
			}

			var planner = agent.GetComponent<GoalOrientedActionPlanner>();
			if (planner != null)
			{
				planner.Initialize();
				_initializedObjects.Push(planner);
				while (planner.InitializationStatus != InitializationState.Active)
					yield return null;
			}

			var layeredPlanner = agent.GetComponent<LayeredGoalOrientedActionPlanner>();
			if (layeredPlanner != null)
			{
				layeredPlanner.Initialize();
				_initializedObjects.Push(layeredPlanner);
				while (layeredPlanner.InitializationStatus != InitializationState.Active)
					yield return null;
			}

			gameScreen.SetActive(true);
			loader.SetActive(false);
		}

		public void End() => StartCoroutine(EndCoroutine());

		private IEnumerator EndCoroutine()
		{
			loader.SetActive(true);
			gameScreen.SetActive(false);

			while (_initializedObjects.Count > 0)
			{
				var current = _initializedObjects.Pop();
				current.Shutdown();
				while (current.InitializationStatus != InitializationState.Inactive)
					yield return null;
			}

			var sceneUnloadOp = SceneManager.UnloadSceneAsync(sceneName);
			while (!sceneUnloadOp.isDone) yield return null;

			mainMenu.SetActive(true);
			loader.SetActive(false);
		}

		private void OnDestroy()
		{
			domain.Shutdown();
			
			template.Shutdown();
		}
	}
}