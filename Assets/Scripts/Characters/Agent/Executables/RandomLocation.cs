using System.Collections.Generic;
using UnityEngine;

public class RandomLocation : MonoBehaviour
{
    private static readonly Dictionary<string, List<RandomLocation>> registry = new Dictionary<string, List<RandomLocation>>();

    [SerializeField] private string id = "";

    private void OnEnable()
    {
	    if (!registry.ContainsKey(id)) registry.Add(id, new List<RandomLocation>());
	    registry[id].Add(this);
    }

    private void OnDisable()
    {
	    registry[id].Remove(this);
	    if (registry[id].Count == 0) registry.Remove(id);
    }

    public static bool TryGet(string keyName, out RandomLocation location)
    {
	    if (!registry.ContainsKey(keyName))
	    {
		    location = null;
		    return false;
	    }
	    
	    var currentRegistry = registry[keyName];
        var registrySize = currentRegistry.Count;

        if (registrySize == 0)
        {
	        location = null;
            return false;
        }

        var randomIndex = Random.Range(0, registrySize);
        location = currentRegistry[randomIndex];
        return true;
    }
}