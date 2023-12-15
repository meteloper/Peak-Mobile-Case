
using System;
using UnityEngine;
using static UnityEditor.Progress;

namespace Metelab.PeakGameCase
{


    [CreateAssetMenu(fileName = "New Grid", menuName = "PeakGameCase/Grid")]
    public class GridSO : MeteScriptableObject
    {
        public int height = 5;
        public int width = 5;
        public int maxMoveCount = 25;
        public GridLayer[] Layers;
        public GridLayer BaseLayer
        {
            get
            {
                return Layers[0];
            }
        }

  

        [Header("Tools")]
        [SerializeField]private string GridItemCreateTypeFromString;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (height <= 0)
            {
                Metelab.LogWarning($"{nameof(height)} should be greater then 0");
                return;
            }
               
            if (width <= 0)
            {
                Metelab.LogWarning($"{nameof(width)} should be greater then 0");
                return;
            }

            if (Layers[0].GridItemsCreateType.Length % height != 0)
            {
                Metelab.LogWarning($"Base layer lenght % {nameof(height)} should be 0");
                return;
            }
                
            if (Layers[0].GridItemsCreateType.Length % width != 0)
            {
                Metelab.LogWarning($"Base layer lenght % {nameof(width)} should be 0");
                return;
            }
                

            if (Layers[0].GridItemsCreateType.Length != height* width)
            {
                Metelab.LogWarning($"Base layer lenght should be equal to {nameof(height)}*{nameof(width)}. ({Layers[0].GridItemsCreateType.Length}:{height * width})");
                return;
            }

            Metelab.Log("Grid data is suitable.", Color.green);

        }


        public int loadTargetLayer;
        public void LoadFromString()
        {
            Layers[loadTargetLayer].GridItemsCreateType = new NodeItemCreateType[GridItemCreateTypeFromString.Length];

            for (int i = 0; i < GridItemCreateTypeFromString.Length; i++)
            {
                int index = int.Parse(GridItemCreateTypeFromString[i].ToString());

                if(index < (int)NodeItemCreateType.MAX)
                {
                    Layers[loadTargetLayer].GridItemsCreateType[i] = (NodeItemCreateType)index;
                }
                else
                {
                    Metelab.LogWarning($"Value = {index}. Values should be less then {(int)NodeItemCreateType.MAX}");
                }
            }
        }

#endif

    }

    [Serializable]
    public struct GridLayer
    {
        public NodeItemCreateType[] GridItemsCreateType;
    }
}
