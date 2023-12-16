using Metelab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metelab.PeakGameCase
{
    public class GridNode : MeteMono, IPointerClickHandler
    {
        public List<NodeItemBase> Items;

        public int x;
        public int y;
        public Action<GridNode> OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }
    }
}
