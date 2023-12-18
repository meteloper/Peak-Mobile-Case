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
                return MainItem != null && MainItem.State == MainItemStates.GROUND;
            }
        }


        public NodeMainItem MainItem
        {
            get
            {
                return (NodeMainItem)Items[0];
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

                value.Ground = RectTransform;
            }
        }

        public bool IsHaveItemInFirstLayer
        {
            get
            {
                return Items!= null && Items.Count > 0 && Items[0] != null; 
            }
        }

        public NodeMainItem TakeMainItem()
        {
            NodeMainItem mainItem = (NodeMainItem)Items[0];
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
