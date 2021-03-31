using System.Collections.Generic;
using UnityEngine;

public class RandomLocation : MonoBehaviour
{
    private static readonly List<RandomLocation> registry = new List<RandomLocation>();
    private void OnEnable() => registry.Add(this);
    private void OnDisable() => registry.Remove(this);

    public static bool TryGet(out Vector3 randomLocation)
    {
        var registrySize = registry.Count;

        if (registrySize == 0)
        {
            randomLocation = Vector3.zero;
            return false;
        }

        var randomIndex = Random.Range(0, registrySize);
        randomLocation = registry[randomIndex].transform.position;
        return true;
    }
}