
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
        public GridGoal[] gridGoals;
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
            Dictionary<Color32,NodeItemCreateTypes> createTypeToColor = new Dictionary<Color32,NodeItemCreateTypes>
            {
                { new Color32(251,242,54,255) ,NodeItemCreateTypes.CUBE_YELLOW },
                { new Color32(99,155,255,255),NodeItemCreateTypes.CUBE_BLUE },
                { new Color32(153,229,80,255),NodeItemCreateTypes.CUBE_GREEN },
                { new Color32(172,50,50,255),NodeItemCreateTypes.CUBE_RED },
                { new Color32(118,66,138,255),NodeItemCreateTypes.CUBE_PURPLE },
                { new Color32(138,111,48,255),NodeItemCreateTypes.CUBE_RANDOM },
                { new Color32(255,255,255,255),NodeItemCreateTypes.EMPTY },
            };

            height = gridTextures[0].height;
            width = gridTextures[0].width;

            layers = new GridLayer[gridTextures.Length];
         

            for (int L = 0; L < gridTextures.Length; L++)
            {
                layers[L].gridItemsCreateType = new NodeItemCreateTypes[width*height];

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
        public NodeItemCreateTypes[] gridItemsCreateType;
    }

    [Serializable]
    public struct GridGoal
    {
        public NodeItemTypes nodeItemType;
        public int count;
    }
}
