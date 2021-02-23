using System.Collections.Generic;

namespace UnityEngine
{
    using Internal;

    public class Tester : MonoBehaviour
    {
        private Queue<Internal.TimerHandle> _container = new Queue<Internal.TimerHandle>();

        private void Callback()
        {
            _container.Dequeue();
        }

        private void OnGUI()
        {
            if (!NativeGameplayCommandBuffer.Instance.HasValue) return;
            var commandBuffer = NativeGameplayCommandBuffer.Instance.Value;

            if (GUILayout.Button("Add"))
            {
                _container.Enqueue(commandBuffer.SetTimer(Callback, 5f));
            }

            if (GUILayout.Button("Pause"))
            {
                foreach (var timerHandle in _container)
                {
                    if (!commandBuffer.IsHandleValid(in timerHandle)) continue;

                    commandBuffer.PauseTimer(in timerHandle);
                }
            }

            if (GUILayout.Button("Resume"))
            {
                foreach (var timerHandle in _container)
                {
                    if (!commandBuffer.IsHandleValid(in timerHandle)) continue;

                    commandBuffer.ResumeTimer(in timerHandle);
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                foreach (var timerHandle in _container)
                {
                    if (!commandBuffer.IsHandleValid(in timerHandle)) continue;

                    commandBuffer.CancelTimer(in timerHandle);
                }
                
                _container.Clear();
            }

            foreach (var timerHandle in _container)
            {
                if (!commandBuffer.IsHandleValid(in timerHandle)) continue;

                var timeRemaining = commandBuffer.GetTimeRemaining(in timerHandle);
                GUILayout.Label($"{timeRemaining}");
            }
        }
    }
}