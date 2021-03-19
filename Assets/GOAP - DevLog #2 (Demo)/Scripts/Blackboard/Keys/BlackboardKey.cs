using System;
using UnityEngine;

namespace LGOAPDemo
{
    public abstract class BlackboardKey : ScriptableObject
    {
        [SerializeField] private bool instanceSynced = false;
        [SerializeField] private bool essentialToDecisionMaking = true;
        [NonSerialized] public int Index = -1;

        public BlackboardKeyTraits Traits
        {
            get
            {
                var output = BlackboardKeyTraits.None;
                if (instanceSynced) output |= BlackboardKeyTraits.InstanceSynced;
                if (essentialToDecisionMaking) output |= BlackboardKeyTraits.EssentialToDecisionMaking;
                return output;
            }
        }

        public abstract byte SizeInBytes { get; }
        public abstract unsafe void SetDefault(byte* value);
        public abstract unsafe string GetValue(byte* data);
    }

    public abstract class BlackboardKey<T> : BlackboardKey where T : unmanaged
    {
        [SerializeField] private T defaultValue = default;

        public sealed override unsafe byte SizeInBytes => (byte) sizeof(T);
        public sealed override unsafe void SetDefault(byte* value) => *(T*) value = defaultValue;
        public sealed override unsafe string GetValue(byte* data) => (*(T*) data).ToString();
    }
}