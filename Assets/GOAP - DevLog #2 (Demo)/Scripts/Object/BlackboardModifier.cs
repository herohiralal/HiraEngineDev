using UnityEngine;

public class BlackboardModifier : MonoBehaviour
{
#if UNITY_EDITOR
#pragma warning disable CS0414
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private HiraBlackboardKeySet keySet = null;
#pragma warning restore CS0414
#endif
    [SerializeField] private SerializableBlackboardModification[] modifications = null;
    private IBlackboardModification[] _modifications = null;

    public void OnBeginPlay()
    {
        _modifications = new IBlackboardModification[modifications.Length];
        for (var i = 0; i < modifications.Length; i++) _modifications[i] = modifications[i].Modification;
    }

    public void Modify(GameObject target)
    {
        var valueAccessor = target.GetComponent<HiraBlackboard>();
        foreach (var modification in _modifications) modification.ApplyTo(valueAccessor);
    }
}