using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{

    [CreateAssetMenu(fileName = "GridNodeItemPrefabs", menuName = "PeakGameCase/Single/GridNodeItemPrefabs")]
    public class GridNodeItemPrefabsSO:MeteSingletonScriptableObject<GridNodeItemPrefabsSO>
    {
        public GridItemBase[] GridItems;
        

        public GridItemBase GetGridItemPrefab(NodeItemType gridItemType)
        {
            return GridItems[(int)gridItemType];
        }


    }
}

