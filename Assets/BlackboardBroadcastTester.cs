using UnityEngine;
using UnityEngine.Internal;
using TimerHandle = UnityEngine.Internal.TimerHandle;

public class BlackboardBroadcastTester : MonoBehaviour
{
    [SerializeField] private Color color = Color.red;
    [SerializeField] private HiraBlackboardComponent blackboard = null;
    private TimerHandle? _handle = null;

    private void Awake()
    {
        blackboard.OnKeyEssentialToDecisionMakingUpdate += SetLightToGreen;
    }

    private void SetLightToGreen()
    {
        color = Color.green;
        if (_handle.HasValue) NativeGameplayCommandBuffer.Instance?.CancelTimer(_handle.Value);
        _handle = NativeGameplayCommandBuffer.Instance?.SetTimer(SetLightToRed, 1f);
    }

    private void SetLightToRed()
    {
        color = Color.red;
        _handle = null;
    }

    private void OnDestroy()
    {
        blackboard.OnKeyEssentialToDecisionMakingUpdate -= SetLightToGreen;
    }
}
