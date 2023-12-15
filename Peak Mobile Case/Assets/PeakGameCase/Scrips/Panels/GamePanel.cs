using DG.Tweening;
using JetBrains.Annotations;
using Metelab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GamePanel : MetePanel
    {
        [Header("GamePanel")]
        public GridSO gridSO;
        public RectTransform GridNodesParent;
        public GridNode GridNodePrefab;

        private GridNode[] ArrayGridNodes;

        public override void Init()
        {
            base.Init();
            CalculateBorderSizeAndPosition();
            CreateGrid();


        }

        private void CalculateBorderSizeAndPosition()
        {
            float nodeWidth = GridNodePrefab.RectTransform.rect.width;
            float nodeHeight = GridNodePrefab.RectTransform.rect.height;
            Vector2 marginOffSet = new Vector2(Constants.GRID_MARGINS.left + Constants.GRID_MARGINS.right, Constants.GRID_MARGINS.bot + Constants.GRID_MARGINS.top);
            GridNodesParent.sizeDelta = new Vector2(gridSO.width * nodeWidth, gridSO.height * nodeHeight) + marginOffSet;
            GridNodesParent.anchoredPosition = Vector2.zero;
        }


        public void CreateGrid()
        {
            ArrayGridNodes = new GridNode[gridSO.GridItemsCreateType.Length];
            float spaceX = GridNodePrefab.RectTransform.rect.width;
            float spaceY = GridNodePrefab.RectTransform.rect.height;
            Vector2 marginOffSet = new Vector2(Constants.GRID_MARGINS.left, Constants.GRID_MARGINS.right);

            for (int y = 0; y < gridSO.height; y++)  
            {
                for (int x = 0; x < gridSO.width; x++)
                {
                    int index = x * gridSO.width + y;
                    ArrayGridNodes[index] = Instantiate(GridNodePrefab, GridNodesParent);
                    ArrayGridNodes[index].name = $"Node[{x},{y}]";
                    ArrayGridNodes[index].RectTransform.anchoredPosition = new Vector2(x * spaceX, y * spaceY) + marginOffSet;
                    GridItemBase gridItem = Instantiate(GridNodeItemPrefabsSO.Instance.GetGridItemPrefab((GridItemType)(int)gridSO.GridItemsCreateType[index]), ArrayGridNodes[index].RectTransform);
                    ArrayGridNodes[index].GridItems.Add(gridItem);
                    gridItem.RectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

   
    }
}

