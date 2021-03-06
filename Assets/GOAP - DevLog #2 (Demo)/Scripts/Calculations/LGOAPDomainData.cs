using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable

namespace LGOAPDemo
{
    public readonly unsafe struct LGOAPDomainData : IDisposable
    {
        public LGOAPDomainData(Allocator allocator, byte goalCount, byte taskCount, params byte[] intermediateGoalCounts)
        {
            var numberOfBytesToAllocate = 0
                                          + (sizeof(byte) * 2) // goal count & task count
                                          + (sizeof(LGOAPGoalCalculationData) * goalCount) // goal calculation data
                                          + (sizeof(LGOAPGoalTargetData) * goalCount) // main goal target data
                                          + (sizeof(LGOAPTaskData) * taskCount) // pure task data
                                          + intermediateGoalCounts.Sum( // for each layer
                                              intermediateGoalCount => 0
                                                                       + 1 // number of intermediate goals (this layer)
                                                                       + (sizeof(LGOAPTaskData) * intermediateGoalCount) // intermediate goal (this layer) task data
                                                                       + (sizeof(LGOAPGoalTargetData) * intermediateGoalCount)); // intermediate goal (this layer) target data

            _container = new NativeArray<byte>(numberOfBytesToAllocate, allocator, NativeArrayOptions.UninitializedMemory);
            ushort allocatedSize = 0;

            // goal count
            ushort toAllocate = 1;
            _container[allocatedSize] = goalCount;
            allocatedSize += toAllocate;

            // primary goal calculation data
            toAllocate = (ushort) (goalCount * sizeof(LGOAPGoalCalculationData));
            allocatedSize += toAllocate;

            // primary goal target data
            toAllocate = (ushort) (goalCount * sizeof(LGOAPGoalTargetData));
            allocatedSize += toAllocate;

            foreach (var intermediateGoalCount in intermediateGoalCounts)
            {
                // intermediate goal count (current layer)
                toAllocate = 1;
                _container[allocatedSize] = intermediateGoalCount;
                allocatedSize += toAllocate;

                // intermediate goal task data (current layer)
                toAllocate = (ushort) (intermediateGoalCount * sizeof(LGOAPTaskData));
                allocatedSize += toAllocate;

                // intermediate goal target data (current layer)
                toAllocate = (ushort) (intermediateGoalCount * sizeof(LGOAPGoalTargetData));
                allocatedSize += toAllocate;
            }

            // task count
            toAllocate = 1;
            _container[allocatedSize] = taskCount;
            allocatedSize += toAllocate;

            // pure task data
            toAllocate = (ushort) (taskCount * sizeof(LGOAPTaskData));
            allocatedSize += toAllocate;

            Assert.AreEqual(numberOfBytesToAllocate, allocatedSize, "Bytes not allocated properly in LGOAPDomainData.");
        }

        private readonly NativeArray<byte> _container;

        public void Dispose() => _container.Dispose();

        public NativeArray<LGOAPGoalCalculationData> GoalCalculationData =>
            _container
                .GetSubArray(1, _container[0] * sizeof(LGOAPGoalCalculationData))
                .Reinterpret<LGOAPGoalCalculationData>(sizeof(byte));

        public NativeArray<LGOAPGoalTargetData> GetTargetDataForLayer(byte layerIndex)
        {
            var sizeOfElementToSkip = layerIndex == 0 ? sizeof(LGOAPGoalCalculationData) : sizeof(LGOAPTaskData);
            var countIndex = layerIndex == 0 ? 0 : GetCountIndex((byte) (layerIndex - 1));
            var count = _container[countIndex];

            return _container
                .GetSubArray(countIndex + 1 + (sizeOfElementToSkip * count), count * sizeof(LGOAPGoalTargetData))
                .Reinterpret<LGOAPGoalTargetData>(sizeof(byte));
        }

        public NativeArray<LGOAPTaskData> GetTaskDataForLayer(byte layerIndex)
        {
            var countIndex = GetCountIndex(layerIndex);

            return _container
                .GetSubArray(countIndex + 1, _container[countIndex] * sizeof(LGOAPTaskData))
                .Reinterpret<LGOAPTaskData>(sizeof(byte));
        }

        private ushort GetCountIndex(byte layerIndex)
        {
            var container = (byte*) _container.GetUnsafeReadOnlyPtr();

            var currentLayerCountIndex = (ushort) (0
                                                   + 1 // goal count
                                                   + (sizeof(LGOAPGoalCalculationData) * container[0]) // main goal calculation data
                                                   + (sizeof(LGOAPGoalTargetData) * container[0])); // main goal target data

            for (var i = 0; i < layerIndex; i++)
            {
                var count = container[currentLayerCountIndex];
                currentLayerCountIndex += (ushort) (0
                                                    + 1 // count
                                                    + (count * sizeof(LGOAPTaskData)) // current task data
                                                    + (count * sizeof(LGOAPGoalTargetData))); // current target data
            }

            return currentLayerCountIndex;
        }
    }
}