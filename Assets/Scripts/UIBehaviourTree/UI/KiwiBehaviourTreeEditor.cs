using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using System;

public class KiwiBehaviourTreeEditor : EditorWindow
{
    KiwiBehaviourTreeView treeView;
    KiwiInspectorView inspectorView;
    IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    [MenuItem("KiwiBehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        KiwiBehaviourTreeEditor wnd = GetWindow<KiwiBehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("KiwiBehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is KiwiBehaviourTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UIBehaviourTree/UI/KiwiBehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UIBehaviourTree/UI/KiwiBehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<KiwiBehaviourTreeView>();
        inspectorView = root.Q<KiwiInspectorView>();
        blackboardView = root.Q<IMGUIContainer>();
        blackboardView.onGUIHandler = () =>
        {
            treeObject.Update();
            EditorGUILayout.PropertyField(blackboardProperty);
            treeObject.ApplyModifiedProperties();
        };

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnSelectionChange()
    {
        KiwiBehaviourTree tree = Selection.activeObject as KiwiBehaviourTree;

        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                KiwiBehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<KiwiBehaviourTreeRunner>();

                if (runner)
                {
                    tree = runner.tree;
                }
            }
        }

        if (Application.isPlaying)
        {
            if (tree)
            {
                treeView.PopulateView(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
            }
        }

        if (tree != null)
        {
            treeObject = new SerializedObject(tree);
            blackboardProperty = treeObject.FindProperty("blackboard");
        }
    }

    void OnNodeSelectionChanged(KiwiNodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}