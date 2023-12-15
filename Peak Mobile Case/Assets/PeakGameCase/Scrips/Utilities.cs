using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    public class Utilities
    {
        public static NodeItemType CreateTypeToItemType(NodeItemCreateType createType)
        {
            if(createType == NodeItemCreateType.CUBE_RANDOM)
            {
                return (NodeItemType)Random.Range(0, (int)NodeItemType.MAX);
            }
            else if(createType == NodeItemCreateType.EMPTY)
            {
                return NodeItemType.NONE;
            }
            else
            {
                return (NodeItemType)createType;
            }
        }
    }
}
