using HiraEngine.Components.Blackboard.Internal;

namespace UnityEngine.Internal
{
    [CreateAssetMenu]
    [HiraCollectionCustomizer(1, RequiredAttributes = new[] {typeof(HiraBlackboardEffectorAttribute)})]
    public class EffectorTester : HiraCollection<IBlackboardEffector>
    {
    }
}