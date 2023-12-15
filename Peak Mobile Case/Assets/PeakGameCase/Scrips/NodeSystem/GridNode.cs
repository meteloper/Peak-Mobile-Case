using Metelab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metelab.PeakGameCase
{
    public class GridNode : MeteMono, IPointerClickHandler
    {
        public List<GridItemBase> GridItems;

        public void OnPointerClick(PointerEventData eventData)
        {
            Metelab.Log(this, name);
        }


    }
}
