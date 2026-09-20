using System.Collections.Generic;
using UnityEngine;

public abstract class KiwiCompositeNode : KiwiNode
{
    [HideInInspector] public List<KiwiNode> children = new List<KiwiNode>();

    public override KiwiNode Clone()
    {
        KiwiCompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}