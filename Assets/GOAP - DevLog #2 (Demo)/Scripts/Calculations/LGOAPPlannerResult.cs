using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LGOAPDemo
{
    public enum LGOAPPlannerResultType : byte
    {
        Uninitialized = 0,
        Failure = 1,
        Unchanged = 2,
        Success = 3
    }

    [BurstCompile]
    public unsafe struct LGOAPPlannerResult : IDisposable
    {
        public LGOAPPlannerResult(byte bufferSize, Allocator allocator) =>
            _container = new NativeArray<byte>(bufferSize + 2, allocator, NativeArrayOptions.UninitializedMemory)
            {
                [0] = 0,
                [1] = (byte) LGOAPPlannerResultType.Uninitialized
            };

        public void Dispose() => _container.Dispose();

        private NativeArray<byte> _container;

        public byte Count
        {
            get => _container[0];
            set => _container[0] = value;
        }

        public LGOAPPlannerResultType ResultType
        {
            get => (LGOAPPlannerResultType) _container[1];
            set => _container[1] = (byte) value;
        }

        public byte BufferSize => (byte) (_container.Length - 2);

        public byte this[byte index]
        {
            get => _container[index + 2];
            set => _container[index + 2] = value;
        }

        [BurstCompile]
        public byte* GetUnsafePtr()
        {
            return 2 + (byte*) _container.GetUnsafePtr();
        }

        [BurstCompile]
        public byte* GetUnsafeReadOnlyPtr()
        {
            return 2 + (byte*) _container.GetUnsafeReadOnlyPtr();
        }

        [BurstCompile]
        public void CopyTo(LGOAPPlannerResult other) => _container.CopyTo(other._container);
    }
}