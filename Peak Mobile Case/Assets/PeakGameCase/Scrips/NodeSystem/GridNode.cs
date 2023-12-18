using Metelab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

namespace Metelab.PeakGameCase
{
    public class GridNode : MeteMono, IPointerClickHandler
    {
        public List<NodeItemBase> Items;
        public int x;
        public int y;
        public Action<GridNode> OnClick;
        public bool IsActive
        {
            get
            {
                return DynamicItem != null && DynamicItem.State == MainItemStates.GROUND;
            }
        }


        public NodeDynamicItem DynamicItem
        {
            get
            {
                return (NodeDynamicItem)Items[0];
            }
            set
            {
                if (Items != null)
                {
                    Items = new List<NodeItemBase>() { value };
                }
                else if (Items.Count > 0)
                {
                    Items[0] = value;
                }

                value.Node = this;
            }
        }

        public bool IsHaveDynamicItem
        {
            get
            {
                return Items!= null && Items.Count > 0 && Items[0] != null; 
            }
        }

        public NodeDynamicItem TakeDynamicItem()
        {
            NodeDynamicItem mainItem = (NodeDynamicItem)Items[0];
            Items[0] = null;
            return mainItem;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsActive)
                OnClick?.Invoke(this);
        }
    }
}
