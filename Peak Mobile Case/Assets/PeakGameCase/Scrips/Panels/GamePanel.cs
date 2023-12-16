using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GamePanel : MetePanel
    {
        [Header("GamePanel")]
        public GridSO gridSO;
        public RectTransform GridNodesParent;
        public RectTransform GridNodesItemParent;
        public GridNode GridNodePrefab;
        public DynamicGridBorder DynamicGridBorder;

        [Header("For Debug")]
        [SerializeField] private GridNode[] ArrayGridNodes;
        [SerializeField] private GameStates currentGameState;
        private int width;
        private int height;

        public override void Init()
        {
            base.Init();
            CalculateGridSizeAndPosition();
            CreateGrid();
            currentGameState = GameStates.CanMove;
        }


        private void CalculateGridSizeAndPosition()
        {
            float nodeWidth = GridNodePrefab.RectTransform.rect.width;
            float nodeHeight = GridNodePrefab.RectTransform.rect.height;
            GridNodesParent.sizeDelta = new Vector2(gridSO.width * nodeWidth, gridSO.height * nodeHeight);
            GridNodesParent.anchoredPosition = Vector2.zero;

            GridNodesItemParent.sizeDelta = GridNodesParent.sizeDelta;
            GridNodesItemParent.anchoredPosition = Vector2.zero;

            DynamicGridBorder.bordersParent.sizeDelta = GridNodesParent.sizeDelta;
            DynamicGridBorder.bordersParent.anchoredPosition = Vector2.zero;
            DynamicGridBorder.SetGridSize(gridSO.width, gridSO.height);
        }

        public void CreateGrid()
        {
            ArrayGridNodes = new GridNode[gridSO.BaseLayer.gridItemsCreateType.Length];
            float spaceX = GridNodePrefab.RectTransform.rect.width;
            float spaceY = GridNodePrefab.RectTransform.rect.height;
            for (int y = 0; y < gridSO.height; y++)  
            {
                for (int x = 0; x < gridSO.width; x++)
                {
                    //Creating Nodes
                    int index = y * gridSO.width + x;

                    if (gridSO.layers[0].gridItemsCreateType[index] == NodeItemCreateTypes.EMPTY)
                        continue;

                    ArrayGridNodes[index] = Instantiate(GridNodePrefab, GridNodesParent);
                    ArrayGridNodes[index].name = $"Node[{x},{y}]";
                    ArrayGridNodes[index].x = x;
                    ArrayGridNodes[index].y = y;    
                    ArrayGridNodes[index].OnClick += OnClickGridNode;
                    ArrayGridNodes[index].RectTransform.anchoredPosition = new Vector2(x * spaceX, y * spaceY);

                    //SettingBorder
                    DynamicGridBorder.SetFilledArea(x, y);

                    //Creating Nodes' Items
                    for (int i = 0; i < gridSO.layers.Length; i++)
                    {
                        NodeItemTypes itemType = Utilities.CreateTypeToItemType(gridSO.layers[i].gridItemsCreateType[index]);
                        Metelab.Log("("+x+","+y+")  "+i+" "+ gridSO.layers[i].gridItemsCreateType[index].ToString()+" "+ itemType);

                        if (itemType == NodeItemTypes.NONE)
                            continue;

                        NodeItemBase itemPrefab = GridNodeItemPrefabsSO.Instance.GetGridItemPrefab(itemType);
                        NodeItemBase gridItem = Instantiate(itemPrefab, GridNodesItemParent);
                        ArrayGridNodes[index].Items.Add(gridItem);
                        gridItem.RectTransform.anchoredPosition = ArrayGridNodes[index].RectTransform.anchoredPosition;
                    }
                }
            }

            DynamicGridBorder.CreateBorder();
        }

        private void OnClickGridNode(GridNode clickGridNode)
        {
            Metelab.Log(this, $"{clickGridNode.name} : Neighbour Nodes Count: {GetSideNeighbourNodes(clickGridNode).Length}" );

            if(currentGameState == GameStates.CanMove)
            {
                NodeItemTypes triggerItemType = NodeItemTypes.NONE;

                if (IsHaveNodeItem(clickGridNode, NodeItemTypes.CUBE_YELLOW))
                {
                    triggerItemType = NodeItemTypes.CUBE_YELLOW;
                }
                else if (IsHaveNodeItem(clickGridNode, NodeItemTypes.CUBE_BLUE))
                {
                    triggerItemType = NodeItemTypes.CUBE_BLUE;
                }
                else if (IsHaveNodeItem(clickGridNode, NodeItemTypes.CUBE_PURPLE))
                {
                    triggerItemType = NodeItemTypes.CUBE_PURPLE;
                }
                else if (IsHaveNodeItem(clickGridNode, NodeItemTypes.CUBE_GREEN))
                {
                    triggerItemType = NodeItemTypes.CUBE_GREEN;
                }
                else if (IsHaveNodeItem(clickGridNode, NodeItemTypes.CUBE_RED))
                {
                    triggerItemType = NodeItemTypes.CUBE_RED;
                }


                if(triggerItemType != NodeItemTypes.NONE)
                {
                    Metelab.Log($"Trigger Item : {triggerItemType}");

                    GridNode[] effectedNodes = FindEffectedNodes(clickGridNode, triggerItemType);


                    if(effectedNodes.Length > 1)
                    {
                        for (int i = 0; i < effectedNodes.Length; i++)
                        {
                            Destroy(effectedNodes[i].Items[0].gameObject);
                            effectedNodes[i].Items.Clear();
                        }
                    }
                }

                currentGameState = GameStates.CanMove;
            }
        }

        private GridNode[] FindEffectedNodes(GridNode node, NodeItemTypes triggerItemType)
        {
            List<GridNode> effectedNodes = new List<GridNode>();
            effectedNodes.Add(node);

            List<GridNode> lastFoundNodes = new List<GridNode>(GetSameItemSideNeighbourNodes(node, triggerItemType));
            effectedNodes.AddRange(lastFoundNodes);

            List<GridNode> currentFoundNodes = new List<GridNode>();


            while (lastFoundNodes.Count > 0)
            {
                for (int i = 0; i < lastFoundNodes.Count; i++)
                {
                    currentFoundNodes = currentFoundNodes.Union(GetSameItemSideNeighbourNodes(lastFoundNodes[i], triggerItemType)).ToList();
                }

                string log = "";
                foreach (var item in currentFoundNodes)
                {
                    log += $"{item.name}, ";
                }
                Metelab.Log(log,Color.red);

                currentFoundNodes = currentFoundNodes.Except(effectedNodes).ToList();
                effectedNodes.AddRange(currentFoundNodes);
                lastFoundNodes = new List<GridNode>(currentFoundNodes);
                
                currentFoundNodes.Clear();


            }

            return effectedNodes.ToArray();
        }

        private GridNode[] GetSameItemSideNeighbourNodes(GridNode node,NodeItemTypes itemType)
        {
            GridNode[] neighbourNode = GetSideNeighbourNodes(node);
            List<GridNode> sameItemNodes = new List<GridNode>();


            foreach (GridNode neighbour in neighbourNode)
            {

                if (IsHaveNodeItem(neighbour, itemType))
                    sameItemNodes.Add(neighbour);
            }

            return sameItemNodes.ToArray();
        }



        private bool IsHaveNodeItem(GridNode node, NodeItemTypes itemType)
        {
            foreach (var item in node.Items)
            {
                if (item.ItemType == itemType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// It is only get up, right, down and left neighbour
        /// </summary>
        private GridNode[] GetSideNeighbourNodes(GridNode node)
        {
            List<GridNode> neighbourNodes = new List<GridNode>();


            GridNode gridNode = GetNeighbourNode(node,Directions.UP);

            if (gridNode != null)
                neighbourNodes.Add(gridNode);

            gridNode = GetNeighbourNode(node, Directions.RIGHT);

            if (gridNode != null)
                neighbourNodes.Add(gridNode);

            gridNode = GetNeighbourNode(node, Directions.DOWN);

            if (gridNode != null)
                neighbourNodes.Add(gridNode);

            gridNode = GetNeighbourNode(node, Directions.LEFT);

            if (gridNode != null)
                neighbourNodes.Add(gridNode);

            return neighbourNodes.ToArray();
        }


        /// <summary>
        /// It can return null
        /// </summary>
        private GridNode GetNeighbourNode(GridNode node, Directions direction)
        {
            GridNode gridNode = null;
            switch (direction)
            {
                case Directions.UP:

                    if (node.y + 1 < gridSO.height)
                        gridNode = ArrayGridNodes[node.x + ((node.y+1) * gridSO.width)];

                    break;
                case Directions.UP_RIGHT:
                    if (node.x + 1 < gridSO.width && node.y + 1 < gridSO.height)
                        gridNode = ArrayGridNodes[(node.x + 1) + ((node.y + 1) * gridSO.width)];

                    break;
                case Directions.RIGHT:
                    if (node.x + 1 < gridSO.width)
                        gridNode = ArrayGridNodes[(node.x + 1) + (node.y * gridSO.width)];

                    break;
                case Directions.DOWN_RIGHT:
                    if (node.x + 1 < gridSO.width && node.y >= 1)
                        gridNode = ArrayGridNodes[(node.x + 1) + ((node.y-1) * gridSO.width)];

                    break;
                case Directions.DOWN:
                    if (node.y >= 1)
                        gridNode = ArrayGridNodes[node.x + ((node.y - 1) * gridSO.width)];

                    break;
                case Directions.DOWN_LEFT:
                    if (node.x >= 1 && node.y >= 1)
                        gridNode = ArrayGridNodes[(node.x - 1) + ((node.y - 1) * gridSO.width)];

                    break;
                case Directions.LEFT:
                    if (node.x >= 1)
                        gridNode = ArrayGridNodes[(node.x - 1) + (node.y * gridSO.width)];
                    break;
                case Directions.UP_LEFT:
                    if (node.x >= 1 && node.y + 1 < gridSO.height)
                        gridNode = ArrayGridNodes[(node.x - 1) + ((node.y + 1) * gridSO.width)];
                    break;
            }

            return gridNode;
        }

        private GridNode[] FindAllEmptyGridNodes()
        {
            return null;
        }

        private IEnumerator BlustAnimations(GridNode target, GridNode[] others)
        {

        

           yield return StartCoroutine(FillAnimations());
        }

        private IEnumerator FillAnimations()
        {


            yield return StartCoroutine(FallAnimations(FindAllEmptyGridNodes()));
        }

        private IEnumerator FallAnimations(GridNode[] gridNodesToFill)
        {


            yield return null;
        }
    }


}

