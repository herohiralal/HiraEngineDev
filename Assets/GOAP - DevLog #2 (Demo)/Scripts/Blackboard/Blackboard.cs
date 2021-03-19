using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace LGOAPDemo
{
    public class Blackboard : MonoBehaviour
    {
        [SerializeField] public BlackboardTemplate template = null;
        [NonSerialized] public NativeArray<byte> Data = default;
        [SerializeField] public bool broadcastKeyUpdateEvents = true;

        public event Action OnKeyEssentialToDecisionMakingUpdate = delegate { };

        public unsafe void Initialize()
        {
            Data = template.GetNewBlackboard();
            template.OnInstanceSyncKeyUpdate += OnInstanceSyncedValueUpdate;
        }

        public unsafe void Shutdown()
        {
            template.OnInstanceSyncKeyUpdate -= OnInstanceSyncedValueUpdate;
            Data.Dispose();
        }

        public T GetValue<T>(string keyName) where T : unmanaged =>
            GetValue<T>(template[keyName]);

        public unsafe T GetValue<T>(int keyIndex) where T : unmanaged =>
            *(T*) ((byte*) Data.GetUnsafeReadOnlyPtr() + keyIndex);

        public void SetValue<T>(string keyName, T value) where T : unmanaged =>
            SetValue<T>(template[keyName], value);

        public unsafe void SetValue<T>(int keyIndex, T value) where T : unmanaged
        {
            var traits = template[keyIndex];
            if (traits.HasFlag(BlackboardKeyTraits.InstanceSynced))
            {
                template.UpdateInstanceSyncedKey(keyIndex, value);
            }
            else
            {
                *(T*) ((byte*) Data.GetUnsafePtr() + keyIndex) = value;

                if (broadcastKeyUpdateEvents && traits.HasFlag(BlackboardKeyTraits.EssentialToDecisionMaking))
                    OnKeyEssentialToDecisionMakingUpdate.Invoke();
            }
        }

        private unsafe void OnInstanceSyncedValueUpdate(int keyIndex, bool isEssentialToDecisionMaking, byte* value, byte size)
        {
            var key = (byte*) Data.GetUnsafePtr() + keyIndex;
            for (var i = 0; i < size; i++) key[i] = value[i];

            if (broadcastKeyUpdateEvents && isEssentialToDecisionMaking)
                OnKeyEssentialToDecisionMakingUpdate.Invoke();
        }
    }
}