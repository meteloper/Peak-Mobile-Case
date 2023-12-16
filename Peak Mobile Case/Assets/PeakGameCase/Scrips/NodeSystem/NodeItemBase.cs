using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metelab.PeakGameCase
{
    public abstract class NodeItemBase : MeteMono
    {
        public NodeItemTypes ItemType;

        public abstract void SetDebugColor();
    }
}
