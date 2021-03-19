using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace LGOAPDemo
{
    [Flags]
    public enum BlackboardKeyTraits : byte
    {
        None = 0,
        InstanceSynced = 1 << 0,
        EssentialToDecisionMaking = 1 << 1
    }

    [CreateAssetMenu]
    public class BlackboardTemplate : HiraCollection<BlackboardKey>
    {
        [NonSerialized] private int _cachedTotalSize = 0;
        [NonSerialized] private Dictionary<string, int> _keyIndices = null;
        [NonSerialized] private Dictionary<int, BlackboardKeyTraits> _keyTraits = null;
        [NonSerialized] private NativeArray<byte> _template = default;

        public int this[string keyName] => _keyIndices[keyName];
        public BlackboardKeyTraits this[int index] => _keyTraits[index];

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
            _keyTraits = new Dictionary<int, BlackboardKeyTraits>();

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
                _keyTraits.Add(index, key.Traits);

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

            _keyTraits = null;
            _keyIndices = null;
        }

        public NativeArray<byte> GetNewBlackboard()
        {
            var output = new NativeArray<byte>(_cachedTotalSize, Allocator.Persistent);
            _template.CopyTo(output);
            return output;
        }

        public unsafe delegate void InstanceSyncKeyUpdateDelegate(int keyIndex, bool isEssentialToDecisionMaking, byte* value, byte size);

        public event InstanceSyncKeyUpdateDelegate OnInstanceSyncKeyUpdate = delegate { };

        public unsafe void UpdateInstanceSyncedKey<T>(int keyIndex, T value) where T : unmanaged
        {
            *(T*) ((byte*) _template.GetUnsafePtr() + keyIndex) = value;

            OnInstanceSyncKeyUpdate.Invoke(
                keyIndex, 
                this[keyIndex].HasFlag(BlackboardKeyTraits.EssentialToDecisionMaking), 
                (byte*) &value, 
                (byte) sizeof(T));
        }
    }
}