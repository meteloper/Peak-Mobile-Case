using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    public class Utilities
    {
        public static NodeItemTypes CreateTypeToItemType(NodeItemCreateTypes createType)
        {
            if(createType == NodeItemCreateTypes.CUBE_RANDOM)
            {
                return (NodeItemTypes)Random.Range(0, (int)NodeItemTypes.MAX);
            }
            else if(createType == NodeItemCreateTypes.EMPTY)
            {
                return NodeItemTypes.NONE;
            }
            else
            {
                return (NodeItemTypes)createType;
            }
        }
    }
}
