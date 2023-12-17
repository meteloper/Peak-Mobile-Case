using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    public class Utilities
    {
        public static NodeItemId CreateTypeToItemType(NodeItemCreateId createType)
        {
            if(createType == NodeItemCreateId.CUBE_RANDOM)
            {
                return (NodeItemId)Random.Range(0, (int)NodeItemId.MAX);
            }
            else if(createType == NodeItemCreateId.SPACE)
            {
                return NodeItemId.NONE;
            }
            else
            {
                return (NodeItemId)createType;
            }
        }
    }
}
