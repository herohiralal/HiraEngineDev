using UnityEditor;
using UnityEngine;

public static class Creator
{
    [MenuItem("GameObject/Hiralal/Smart Object", false, priority = 10)]
    private static void CreateSmartObject(MenuCommand menuCommand)
    {
        var go = new GameObject("New Smart Object");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create Smart Object" + go.name);
        Selection.activeObject = go;
        
        var sphereCollider = go.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 40;

        go.AddComponent<ActionBroadcaster>();
        go.AddComponent<ReportOnActionExecution>();
        go.AddComponent<BlackboardModifier>();
    }
}