using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class HelloWorld : EditorWindow
{
    [MenuItem("Window/UI Toolkit/HelloWorld _#%T")]
    public static void ShowExample()
    {
        var wnd = GetWindow<HelloWorld>();
        wnd.minSize = new Vector2(200, 200);
        wnd.titleContent = new GUIContent("HelloWorld");
    }

    [MenuItem("Window/UI Toolkit/Close _#&T")]
    public static void CloseWindow()
    {
        var wnd = GetWindow<HelloWorld>();
        if (wnd != null) wnd.Close();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        var root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIElements/Editor/HelloWorld.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIElements/Editor/HelloWorld.uss");
        
        VisualElement labelFromUXML = visualTree.Instantiate();
        labelFromUXML.styleSheets.Add(styleSheet);
        root.Add(labelFromUXML);

        root.Q<VisualElement>("Container").style.height = new StyleLength(position.height);
    }
}