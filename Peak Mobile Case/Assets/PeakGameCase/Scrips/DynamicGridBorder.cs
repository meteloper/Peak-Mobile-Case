using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Metelab.PeakGameCase
{
    public class DynamicGridBorder : MonoBehaviour
    {
       // private static MeteObjectPool<DynamicGridBorderNode> borderPool = new MeteObjectPool<DynamicGridBorderNode>();

        public RectTransform bordersParent;
        public int Width;
        public int Height;
        public bool[] filledPlaces;
        public int NodeScaleSize = 2;
        public int BorderNodeSize = 50;

        [Header("Prefabs")]
        [SerializeField] private GameObject prefabUp;
        [SerializeField] private GameObject prefabUpRight;
        [SerializeField] private GameObject prefabRight;
        [SerializeField] private GameObject prefabDownRight;
        [SerializeField] private GameObject prefabDown;
        [SerializeField] private GameObject prefabDownLeft;
        [SerializeField] private GameObject prefabLeft;
        [SerializeField] private GameObject prefabUpLeft;
        [SerializeField] private GameObject prefabInUpRight;
        [SerializeField] private GameObject prefabInDownRight;
        [SerializeField] private GameObject prefabInDownLeft;
        [SerializeField] private GameObject prefabInUpLeft;

        public void SetGridSize(int witdh, int height)
        {
            Width = (witdh * NodeScaleSize)+2;
            Height = (height * NodeScaleSize)+2;
            filledPlaces = new bool[Width * Height];
        }

        public void SetFilledArea(int x, int y)
        {
            int relativeX = (x * NodeScaleSize) + 1;
            int relativeY = (y * NodeScaleSize) + 1;
            int rightX = relativeX + 1;
            int upY = relativeY + 1;

            filledPlaces[relativeX + (relativeY * Width)] = true;
            filledPlaces[rightX + (relativeY * Width)] = true;
            filledPlaces[relativeX + (upY * Width)] = true;
            filledPlaces[rightX + (upY * Width)] = true;
        }



        public void CreateBorder()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = x + (y * Width);

                    if (!filledPlaces[index])// is Empty
                    {
                        bool[] neighborFilledPlace = new bool[8];//For each side, start with Up and goes to clockwise Up, Up-Right, Right ...

                        neighborFilledPlace[0] = IsPlaceFilled(x, y + 1);     //UP
                        neighborFilledPlace[1] = IsPlaceFilled(x + 1, y + 1); //UP-RIGTH
                        neighborFilledPlace[2] = IsPlaceFilled(x + 1, y);     //RIGHT    
                        neighborFilledPlace[3] = IsPlaceFilled(x + 1, y - 1); //DOWN-RIGHT
                        neighborFilledPlace[4] = IsPlaceFilled(x, y - 1);     //DOWN
                        neighborFilledPlace[5] = IsPlaceFilled(x - 1, y - 1); //DOWN-LEFT
                        neighborFilledPlace[6] = IsPlaceFilled(x - 1, y);     //LEFT
                        neighborFilledPlace[7] = IsPlaceFilled(x - 1, y + 1); //UP-LEFT

                        //All neighbor is Empty
                        if(!neighborFilledPlace[0] && !neighborFilledPlace[1] && !neighborFilledPlace[2] && !neighborFilledPlace[3] && !neighborFilledPlace[4] && !neighborFilledPlace[5] && !neighborFilledPlace[6] && !neighborFilledPlace[7])
                            continue;
                        else if (neighborFilledPlace[0] && neighborFilledPlace[1] && neighborFilledPlace[2])
                           CreateBorderNode(prefabInUpRight, x, y);
                        else if (neighborFilledPlace[6] && neighborFilledPlace[7] && neighborFilledPlace[0])
                            CreateBorderNode(prefabInUpLeft, x, y);
                        else if (neighborFilledPlace[2] && neighborFilledPlace[3] && neighborFilledPlace[4])
                            CreateBorderNode(prefabInDownRight, x, y);
                        else if (neighborFilledPlace[4] && neighborFilledPlace[5] && neighborFilledPlace[6])
                            CreateBorderNode(prefabInDownLeft, x, y);
                        else if (neighborFilledPlace[0] && !neighborFilledPlace[2] && !neighborFilledPlace[4] && !neighborFilledPlace[6])
                            CreateBorderNode(prefabUp, x, y);
                        else if (!neighborFilledPlace[0] && neighborFilledPlace[2] && !neighborFilledPlace[4] && !neighborFilledPlace[6])
                            CreateBorderNode(prefabRight, x, y);
                        else if (!neighborFilledPlace[0] && !neighborFilledPlace[2] && neighborFilledPlace[4] && !neighborFilledPlace[6])
                            CreateBorderNode(prefabDown, x, y);
                        else if (!neighborFilledPlace[0] && !neighborFilledPlace[2]! && !neighborFilledPlace[4] && neighborFilledPlace[6])
                            CreateBorderNode(prefabLeft, x, y);
                        else if (neighborFilledPlace[1])
                            CreateBorderNode(prefabUpRight, x, y);
                        else if (neighborFilledPlace[3])
                            CreateBorderNode(prefabDownRight, x, y);
                        else if (neighborFilledPlace[5])
                            CreateBorderNode(prefabDownLeft, x, y);
                        else if (neighborFilledPlace[7])
                            CreateBorderNode(prefabUpLeft, x, y);
                    }
                }
            }
            
        }

        private bool IsPlaceFilled(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return filledPlaces[x + (y * Width)];
            else
                return false;
        }
        private void CreateBorderNode(GameObject prefab, int x, int y)
        {
            RectTransform borderNodeRect = Instantiate(prefab, bordersParent).GetComponent<RectTransform>();
            borderNodeRect.anchoredPosition =  new Vector2(BorderNodeSize * (x-1), BorderNodeSize * (y-1));
        }
    }


}
