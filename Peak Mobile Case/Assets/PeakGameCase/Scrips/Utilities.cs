using UnityEngine;


namespace Metelab.PeakGameCase
{
    public class Utilities
    {
        public static NodeItemIds CreateTypeToItemType(NodeItemCreateId createType)
        {
            if(createType == NodeItemCreateId.CUBE_RANDOM)
            {
                return (NodeItemIds)Random.Range((int)NodeItemIds.CUBE_YELLOW, Constants.CUBE_COUNT);
            }
            else if(createType == NodeItemCreateId.ROCKET_RANDOM)
            {
                return Random.Range(0,2) == 0 ? NodeItemIds.ROCKET_HORIZONTAL : NodeItemIds.ROCKET_VERTICAL;
            }
            else if(createType == NodeItemCreateId.RANDOM)
            {
                return (NodeItemIds)Random.Range(0, (int)NodeItemIds.MAX);
            }
            else
            {
                return (NodeItemIds)createType;
            }
        }
    }
}
