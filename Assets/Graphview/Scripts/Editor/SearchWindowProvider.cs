using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace Graphview.Scripts.Editor
{
    public static class SearchWindowProvider
    {
        public static Searcher GetSearcher()
        {
            var items = new List<SearcherItem>(2)
            {
                new SearcherItem("Dialogue"),
                new SearcherItem("Response")
            };

            var database = SearcherDatabase.Create(items, string.Empty, false);
            return new Searcher(database, new SearchWindowAdapter("Create Node"));
        }

        public static bool OnSearcherSelectEntry(EditorWindow window,SearcherItem entry, Vector2 screenMousePosition, DialogueGraphView dialogueGraphView)
        {
            if (window == null || dialogueGraphView == null) return true;
            
            var windowRoot = window.rootVisualElement;
            var windowMousePos = windowRoot.ChangeCoordinatesTo(windowRoot.parent, screenMousePosition);
            var graphMousePos = dialogueGraphView.contentViewContainer.WorldToLocal(windowMousePos);
            
            switch (entry.Name)
            {
                case "Dialogue":
                    dialogueGraphView.CreateDialogueNode(graphMousePos);
                    break;
                case "Response":
                    dialogueGraphView.CreateResponseNode(graphMousePos);
                    break;
            }

            return true;
        }
    }

    public class SearchWindowAdapter : SearcherAdapter
    {
        private readonly VisualTreeAsset _defaultItemTemplate;
        public override bool HasDetailsPanel => false;

        public SearchWindowAdapter(string title) : base(title)
        {
            _defaultItemTemplate = Resources.Load<VisualTreeAsset>("SearcherItem");
        }
    }
}