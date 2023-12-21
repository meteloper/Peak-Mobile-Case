using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{

    [CreateAssetMenu(fileName = "NodeItemPrefabs", menuName = "PeakGameCase/Single/NodeItemPrefabs")]
    public class NodeItemPrefabsSO:MeteSingletonScriptableObject<NodeItemPrefabsSO>
    {
        public NodeItem[] NodeItems;

        public NodeItem this[NodeItemIds itemId]
        {
            get { return NodeItems[(int)itemId]; }
        }
    }
}

