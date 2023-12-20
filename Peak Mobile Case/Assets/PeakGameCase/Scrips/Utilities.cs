using UnityEngine;


namespace Metelab.PeakGameCase
{
    public static class Utilities
    {
        public static NodeItemIds CreateTypeToItemType(NodeItemCreateId createType)
        {
            if(createType == NodeItemCreateId.RANDOM_CUBE)
            {
                return (NodeItemIds)Random.Range((int)NodeItemIds.CUBE_YELLOW, Constants.CUBE_COUNT);
            }
            else if(createType == NodeItemCreateId.RANDOM_ROCKET)
            {
                return Random.Range(0,2) == 0 ? NodeItemIds.ROCKET_HORIZONTAL : NodeItemIds.ROCKET_VERTICAL;
            }
            else if(createType == NodeItemCreateId.RANDOM_ALL)
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
