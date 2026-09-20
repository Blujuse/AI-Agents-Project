using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class KiwiInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<KiwiInspectorView, VisualElement.UxmlTraits> { }

    Editor editor;

    public KiwiInspectorView()
    {

    }

    internal void UpdateSelection(KiwiNodeView nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() => { 
            if(editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        Add(container);
    }
}