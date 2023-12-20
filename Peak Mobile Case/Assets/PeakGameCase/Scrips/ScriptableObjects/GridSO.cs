
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

namespace Metelab.PeakGameCase
{


    [CreateAssetMenu(fileName = "New Grid", menuName = "PeakGameCase/Grid")]
    public class GridSO : MeteScriptableObject
    {
        public int height = 5;
        public int width = 5;
        public int maxMoveCount = 25;
        public GoalSO[] Goals;
        public GridLayer[] layers;
 
        public GridLayer BaseLayer
        {
            get
            {
                return layers[0];
            }
        }



        [Header("Tools")]
        [SerializeField] private Texture2D[] gridTextures;

#if UNITY_EDITOR
       
        public void LoadFromString()
        {
            Dictionary<Color32,NodeItemCreateIds> createTypeToColor = new Dictionary<Color32,NodeItemCreateIds>
            {
                { new Color32(251,242,54,255) ,NodeItemCreateIds.CUBE_YELLOW },
                { new Color32(99,155,255,255),NodeItemCreateIds.CUBE_BLUE },
                { new Color32(153,229,80,255),NodeItemCreateIds.CUBE_GREEN },
                { new Color32(172,50,50,255),NodeItemCreateIds.CUBE_RED },
                { new Color32(118,66,138,255),NodeItemCreateIds.CUBE_PURPLE },
                { new Color32(138,111,48,255),NodeItemCreateIds.RANDOM_CUBE },
                { new Color32(255,255,255,255),NodeItemCreateIds.SPACE },
                { new Color32(215,123,186,255), NodeItemCreateIds.BALLOON},
                { new Color32(63,63,116,255), NodeItemCreateIds.ROCKET_HORIZONTAL},
                { new Color32(55,148,110,255), NodeItemCreateIds.ROCKET_VERTICAL},
                { new Color32(48,96,130,255), NodeItemCreateIds.RANDOM_ROCKET},
                { new Color32(34,32,52,255), NodeItemCreateIds.DUCK},
                { new Color32(0,0,0,255), NodeItemCreateIds.RANDOM_ALL},
            };

            height = gridTextures[0].height;
            width = gridTextures[0].width;

            layers = new GridLayer[gridTextures.Length];
         

            for (int L = 0; L < gridTextures.Length; L++)
            {
                layers[L].gridItemsCreateType = new NodeItemCreateIds[width*height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        layers[L].gridItemsCreateType[x+(y* width)] = createTypeToColor[gridTextures[L].GetPixel(x, y)];
                    }
                }
            }

            Metelab.Log("Grid loading was succesful", Color.green);
        }

#endif

    }

    [Serializable]
    public struct GridLayer
    {
        public NodeItemCreateIds[] gridItemsCreateType;
    }
}
