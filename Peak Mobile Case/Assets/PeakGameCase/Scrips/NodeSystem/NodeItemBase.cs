using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metelab.PeakGameCase
{
    public abstract class NodeItemBase : MeteMono
    {
        public NodeItemId ItemId;
        public NodeItemType ItemType;

        public abstract void Trigger();
        public abstract void Explode();
    }
}
