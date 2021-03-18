using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace LGOAPDemo
{
	public class BlackboardTemplate : HiraCollection<BlackboardKey>
	{
		[NonSerialized] private int _cachedTotalSize = 0;
		[NonSerialized] private Dictionary<string, int> _keyIndices = null;
		[NonSerialized] private Dictionary<int, bool> _instanceSyncData = null;
		[NonSerialized] private NativeArray<byte> _template = default;

		private int TotalSize
		{
			get
			{
				var size = 0;
				foreach (var key in Collection1)
				{
					size += key.SizeInBytes;
				}

				return size;
			}
		}

		public unsafe void Initialize()
		{
			_keyIndices = new Dictionary<string, int>();
			_instanceSyncData = new Dictionary<int, bool>();

			_cachedTotalSize = TotalSize;
			var sortedKeys = Collection1.OrderBy(k => k.SizeInBytes);
			_template = new NativeArray<byte>(_cachedTotalSize, Allocator.Persistent);
			var templatePtr = (byte*) _template.GetUnsafePtr();

			var index = 0;
			foreach (var key in sortedKeys)
			{
				var keyName = key.name;

				// cache the string-to-id hash table
				if (!_keyIndices.ContainsKey(keyName))
					_keyIndices.Add(keyName, index);
				else
				{
					Debug.LogError($"Blackboard contains multiple keys named {keyName}.", this);
					_keyIndices[keyName] = index;
				}

				// update the index on the key itself
				key.Index = index;

				// update instance syncing data
				_instanceSyncData.Add(index, key.InstanceSynced);

				// set the default value
				key.SetDefault(templatePtr + index);

				// get the next index
				index += key.SizeInBytes;
			}

			Assert.AreEqual(_cachedTotalSize, index);
		}

		public void Shutdown()
		{
			_template.Dispose();

			_instanceSyncData = null;
			_keyIndices = null;
		}
	}
}