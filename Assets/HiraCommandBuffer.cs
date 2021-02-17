using System;
using System.Collections.Generic;
using UnityEngine;

[HiraManager]
public class HiraCommandBuffer : MonoBehaviour
{
	public static HiraCommandBuffer Instance { get; [JetBrains.Annotations.UsedImplicitly] private set; }
	private struct ActiveTimer
	{
		internal bool _paused;
		internal float _timeRemaining;
		internal ulong _hash;
		internal Action _action;
	}
	
	[SerializeField] private ushort bufferSize = 10;
	[SerializeField] private BoolReference gamePaused = null;

	private ActiveTimer[] _commandBuffer;
	private Stack<int> _unusedCommandBufferIndices;
	private ulong _hash;

	private void Awake()
	{
		_hash = 1;
		
		_unusedCommandBufferIndices = new Stack<int>(bufferSize);
		for (var i = 0; i < bufferSize; i++) _unusedCommandBufferIndices.Push(i);

		_commandBuffer = new ActiveTimer[bufferSize];
	}

	private void OnDestroy()
	{
		_commandBuffer = null;
		
		_unusedCommandBufferIndices = null;

		_hash = 1;
	}

	private void Update()
	{
		var count = bufferSize;
		var deltaTime = Time.deltaTime;

		for (var i = 0; i < count; i++)
		{
			_commandBuffer[i]._timeRemaining -= 
		}
	}

	public TimerHandle SetTimerTiedToPauseSystem(Action action, float timer)
	{
		
	}
}

public readonly struct TimerHandle
{
	private readonly WeakReference<HiraCommandBuffer> _owner;

	private readonly ushort _bufferIndex;
	private readonly ulong _hash;

	public float TimeRemaining
	{
		get
		{
			
		}
	}
	
	public void Resume()
	{
	}
	
	public void Pause()
	{
	}

	public void Cancel()
	{
	}
}