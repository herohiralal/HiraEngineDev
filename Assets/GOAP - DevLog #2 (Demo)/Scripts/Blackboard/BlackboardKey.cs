using System;
using UnityEngine;

namespace LGOAPDemo
{
	public abstract class BlackboardKey : ScriptableObject
	{
		[SerializeField] private bool instanceSynced = false;
		[NonSerialized] public int Index;

		public bool InstanceSynced => instanceSynced;
		public abstract byte SizeInBytes { get; }
		public abstract unsafe void SetDefault(byte* value);
	}
}