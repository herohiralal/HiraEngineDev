using HiraEngine.Components.Blackboard.Internal;

namespace UnityEngine.Internal
{
    [CreateAssetMenu]
    [HiraCollectionCustomizer(1, RequiredAttributes = new[] {typeof(HiraBlackboardScoreCalculatorAttribute)})]
    public class ScoreCalculatorTester : HiraCollection<IBlackboardScoreCalculator>
    {
    }
}