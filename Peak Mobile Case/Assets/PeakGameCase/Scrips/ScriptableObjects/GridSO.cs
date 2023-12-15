
using UnityEngine;
using static UnityEditor.Progress;

namespace Metelab.PeakGameCase
{


    [CreateAssetMenu(fileName = "New Grid", menuName = "PeakGameCase/Grid")]
    public class GridSO : MeteScriptableObject
    {
        public int height = 5;
        public int width = 5;
        public GridItemCreateType[] GridItemsCreateType;

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

            if (GridItemsCreateType.Length % height != 0)
            {
                Metelab.LogWarning($"{nameof(GridItemsCreateType)}.Lenght % {nameof(height)} should be 0");
                return;
            }
                

            if (GridItemsCreateType.Length % width != 0)
            {
                Metelab.LogWarning($"{nameof(GridItemsCreateType)}.Lenght % {nameof(width)} should be 0");
                return;
            }
                

            if (GridItemsCreateType.Length != height* width)
            {
                Metelab.LogWarning($"{nameof(GridItemsCreateType)}.Lenght should be equal to {nameof(height)}*{nameof(width)}. ({GridItemsCreateType.Length}:{height * width})");
                return;
            }

            Metelab.Log("Grid data is suitable.", Color.green);

        }

        public void LoadFromString()
        {
            GridItemsCreateType = new GridItemCreateType[GridItemCreateTypeFromString.Length];

            for (int i = 0; i < GridItemCreateTypeFromString.Length; i++)
            {
                int index = int.Parse(GridItemCreateTypeFromString[i].ToString());

                if(index < (int)GridItemCreateType.MAX)
                {
                    GridItemsCreateType[i] = (GridItemCreateType)index;
                }
                else
                {
                    Metelab.LogWarning($"Value = {index}. Values should be less then {(int)GridItemCreateType.MAX}");
                }
            }
        }

#endif

    }
}
