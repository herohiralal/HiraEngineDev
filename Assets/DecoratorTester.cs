using HiraEngine.Components.Blackboard.Internal;

namespace UnityEngine.Internal
{
    [CreateAssetMenu]
    [HiraCollectionCustomizer(1, RequiredAttributes = new[] {typeof(HiraBlackboardDecoratorAttribute)})]
    public class DecoratorTester : HiraCollection<IBlackboardDecorator>
    {
    }
}