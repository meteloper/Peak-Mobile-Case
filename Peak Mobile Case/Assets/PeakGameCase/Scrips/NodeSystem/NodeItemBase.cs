using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metelab.PeakGameCase
{
    public abstract class NodeItemBase : MeteMono
    {
        public GridNode Node { get; set; }

        public ExplodeConditions ExplodeCondition;
        public NodeItemIds ItemId;
        public NodeItemTypes ItemType;

        public Action<NodeItemBase> OnTriggered;
        public abstract void Explode(ExplodeConditions triggerType);

    }
}
