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

    public unsafe struct LGOAPPlannerResult : IDisposable
    {
        public LGOAPPlannerResult(byte bufferSize, Allocator allocator) =>
            Container = new NativeArray<byte>(bufferSize + 2, allocator, NativeArrayOptions.UninitializedMemory)
            {
                [0] = 0,
                [1] = (byte) LGOAPPlannerResultType.Uninitialized
            };

        public void Dispose() => Container.Dispose();

        public NativeArray<byte> Container;

        public byte Count
        {
            get => Container[0];
            set => Container[0] = value;
        }

        public LGOAPPlannerResultType ResultType
        {
            get => (LGOAPPlannerResultType) Container[1];
            set => Container[1] = (byte) value;
        }

        public byte BufferSize => (byte) (Container.Length - 2);

        public byte this[byte index]
        {
            get => Container[index + 2];
            set => Container[index + 2] = value;
        }

        [BurstCompile]
        public byte* GetUnsafePtr()
        {
            return 2 + (byte*) Container.GetUnsafePtr();
        }

        [BurstCompile]
        public byte* GetUnsafeReadOnlyPtr()
        {
            return 2 + (byte*) Container.GetUnsafeReadOnlyPtr();
        }

        public LGOAPPlan ToPlan(Allocator allocator)
        {
            var count = Count;

            var plan = new LGOAPPlan(BufferSize, allocator) {Count = count};

            NativeArray<byte>.Copy(Container, 2, plan.Container, 1, count);

            return plan;
        }
    }
}