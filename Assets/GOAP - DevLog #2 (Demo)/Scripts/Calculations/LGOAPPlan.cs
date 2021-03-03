using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace LGOAPDemo
{
    [BurstCompile]
    public unsafe struct LGOAPPlan : IDisposable
    {
        public LGOAPPlan(byte bufferSize, Allocator allocator) =>
            Container = new NativeArray<byte>(bufferSize + 1, allocator, NativeArrayOptions.UninitializedMemory);

        [BurstCompile]
        public void Dispose() => Container.Dispose();

        public NativeArray<byte> Container;

        public byte Count
        {
            [BurstCompile]
            get => Container[0];
            [BurstCompile]
            set => Container[0] = value;
        }

        public byte BufferSize
        {
            [BurstCompile]
            get => (byte) (Container.Length - 1);
        }

        public byte this[byte index]
        {
            [BurstCompile]
            get => Container[index + 1];
            [BurstCompile]
            set => Container[index + 1] = value;
        }

        [BurstCompile]
        public byte* GetUnsafePtr()
        {
            return 1 + (byte*) Container.GetUnsafePtr();
        }

        [BurstCompile]
        public byte* GetUnsafeReadOnlyPtr()
        {
            return 1 + (byte*) Container.GetUnsafeReadOnlyPtr();
        }

        [BurstCompile]
        public LGOAPPlannerResult ToResult(LGOAPPlannerResultType resultType, Allocator allocator)
        {
            var count = Count;

            var result = new LGOAPPlannerResult(BufferSize, allocator) {Count = count, ResultType = resultType};

            NativeArray<byte>.Copy(Container, 1, result.Container, 2, count);

            return result;
        }
    }
}