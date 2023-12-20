using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public static class GameEvents
    {
        public static event Action<NodeItem> OnExplodedNodeItem;

        public static void InvokeNodeItemExplode(NodeItem item)
        {
            OnExplodedNodeItem?.Invoke(item);
        }
    }
}
