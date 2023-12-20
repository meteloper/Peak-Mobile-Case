using Metelab;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

namespace Metelab.PeakGameCase
{
    public class GridNode : MeteMono, IPointerClickHandler
    {
        public List<NodeItem> Items;
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


        public NodeItemDynamic DynamicItem
        {
            get
            {
                return (NodeItemDynamic)Items[0];
            }
            set
            {
                if (Items != null)
                {
                    Items = new List<NodeItem>() { value };
                }
                else if (Items.Count > 0)
                {
                    Items[0] = value;
                }

                value.GridNode = this;
            }
        }

        public void AddItem(NodeItem item)
        {
            if (Items != null)
            {
                Items = new List<NodeItem>() { item };
            }
            else
                Items.Add(item);
            
            item.GridNode = this;
        }

        public bool IsHaveDynamicItem
        {
            get
            {
                return Items!= null && Items.Count > 0 && Items[0] != null; 
            }
        }

        public NodeItemDynamic TakeDynamicItem()
        {
            NodeItemDynamic mainItem = (NodeItemDynamic)Items[0];
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
