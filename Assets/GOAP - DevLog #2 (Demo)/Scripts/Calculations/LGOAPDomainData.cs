using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace LGOAPDemo
{
    public unsafe struct LGOAPDomainData : IDisposable
    {
        public LGOAPDomainData(Allocator allocator, byte goalCount, byte taskCount, params byte[] intermediateGoalCounts)
        {
            var numberOfBytesToAllocate = 0 + (sizeof(byte) * 2) // goal count & task count
                                            + (sizeof(byte) * 1) // intermediate layer count
                                            + (sizeof(LGOAPGoalCalculationData) * goalCount) // goal calculation data
                                            + (sizeof(LGOAPGoalTargetData) * goalCount) // main goal target data
                                            + (sizeof(LGOAPTaskData) * taskCount) // pure task data
                                            + intermediateGoalCounts.Sum( // for each layer
                                                intermediateGoalCount => 0
                                                                         + 1 // number of intermediate goals (this layer)
                                                                         + (sizeof(LGOAPTaskData) * intermediateGoalCount) // intermediate goal (this layer) task data
                                                                         + (sizeof(LGOAPGoalTargetData) * intermediateGoalCount)); // intermediate goal (this layer) target data

            _container = new NativeArray<byte>(numberOfBytesToAllocate, allocator, NativeArrayOptions.UninitializedMemory);
            var allocatedSize = 0;

            // goal count
            var toAllocate = 1;
            _container[allocatedSize] = goalCount;
            allocatedSize += toAllocate;

            // primary goal calculation data
            toAllocate = sizeof(LGOAPGoalCalculationData) * goalCount;
            var goalCalculationData = _container
                .GetSubArray(allocatedSize, toAllocate)
                .Reinterpret<LGOAPGoalCalculationData>(sizeof(byte));
            allocatedSize += toAllocate;

            // primary goal target data
            toAllocate = sizeof(LGOAPGoalTargetData) * goalCount;
            var goalTargetData = _container
                .GetSubArray(allocatedSize, toAllocate)
                .Reinterpret<LGOAPGoalTargetData>(sizeof(byte));
            allocatedSize += toAllocate;

            // layer count
            toAllocate = 1;
            _container[allocatedSize] = (byte) intermediateGoalCounts.Length;
            allocatedSize += toAllocate;
            
            foreach (var intermediateGoalCount in intermediateGoalCounts)
            {
                // intermediate goal count (current layer)
                toAllocate = 1;
                _container[allocatedSize] = intermediateGoalCount;
                allocatedSize += toAllocate;

                // intermediate goal task data (current layer)
                toAllocate = sizeof(LGOAPTaskData) * intermediateGoalCount;
                var intermediateGoalTaskData = _container
                    .GetSubArray(allocatedSize, toAllocate)
                    .Reinterpret<LGOAPTaskData>(sizeof(byte));
                allocatedSize += toAllocate;

                // intermediate goal target data (current layer)
                toAllocate = sizeof(LGOAPGoalTargetData) * intermediateGoalCount;
                var intermediateGoalTargetData = _container
                    .GetSubArray(allocatedSize, toAllocate)
                    .Reinterpret<LGOAPGoalTargetData>(sizeof(byte));
                allocatedSize += toAllocate;
            }
            
            // task count
            toAllocate = 1;
            _container[allocatedSize] = taskCount;
            allocatedSize += toAllocate;
            
            // pure task data
            toAllocate = sizeof(LGOAPTaskData) * taskCount;
            var pureTaskData = _container
                .GetSubArray(allocatedSize, toAllocate)
                .Reinterpret<LGOAPTaskData>(sizeof(byte));
            allocatedSize += toAllocate;

            Assert.AreEqual(numberOfBytesToAllocate, allocatedSize, "Bytes not allocated properly in LGOAPDomainData.");
        }

        private NativeArray<byte> _container;

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}