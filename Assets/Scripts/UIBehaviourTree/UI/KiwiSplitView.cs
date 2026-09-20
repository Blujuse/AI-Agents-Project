using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class KiwiSplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<KiwiSplitView, TwoPaneSplitView.UxmlTraits> { }
}
