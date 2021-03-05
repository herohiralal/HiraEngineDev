using System;
using System.Text;
using Unity.Collections;
using UnityEngine;

namespace LGOAPDemo
{
    [Serializable]
    public class LGOAPPlan<T> where T : ScriptableObject, IHiraCollectionAwareTarget
    {
        private readonly T[] _collection = null;
        [SerializeField] private T[] tasks = null;
        private byte _planSize = 0;
        private short _currentIndex = 0;

        public LGOAPPlan(T[] collection, byte length) => (_collection, tasks) = (collection, new T[length]);

        public static LGOAPPlan<T> From(LGOAPPlannerResult plannerResult, T[] collection, byte maxLength)
        {
            var plan = new LGOAPPlan<T>(collection, maxLength);
            plan.Consume(plannerResult);
            return plan;
        }

        public unsafe void Consume(LGOAPPlannerResult plannerResult)
        {
            _planSize = plannerResult.Count;

            var itStart = plannerResult.GetUnsafeReadOnlyPtr();
            var itMax = itStart + _planSize;

            for (byte i = 0; itStart + i < itMax; i++)
                tasks[i] = _collection[itStart[i]];

            _currentIndex = (short) (_planSize - 1);
        }

        public unsafe LGOAPPlannerResult ToPlannerResult(Allocator allocator, LGOAPPlannerResultType resultType = LGOAPPlannerResultType.Uninitialized)
        {
            var result = new LGOAPPlannerResult(_planSize, allocator);
            var itStart = result.GetUnsafePtr();
            var itMax = itStart + _planSize;

            for (byte i = 0; itStart + i < itMax; i++)
                itStart[i] = (byte) tasks[i].Index;

            result.ResultType = resultType;
            result.Count = _planSize;

            return result;
        }

        public T Pop() => tasks[_currentIndex--];
        public bool HasActions => _currentIndex > -1;
        public void Invalidate() => _currentIndex = -1;
        public void Restart() => _currentIndex = (short) (_planSize - 1);

        public override string ToString()
        {
            var data = new StringBuilder(500);
            for (var i = _planSize - 1; i > -1; i--)
            {
                data.Append(tasks[i].name);
                data.AppendLine(_currentIndex + 1 == i ? " <--" : "");
            }

            return data.ToString();
        }
    }
}