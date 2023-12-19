using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks.Sources;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Metelab.PeakGameCase
{
    public class GamePanel : MetePanel
    {
        private static MeteObjectPool<NodeItemBase> OPNodeItem = new MeteObjectPool<NodeItemBase>();

        [Header("GamePanel")]
        public GridSO gridSO;
        public RectTransform FallStartPoint;
        public RectTransform GridNodesParent;
        public RectTransform GridNodesItemParent;
        public GridNode GridNodePrefab;
        public DynamicGridBorder DynamicGridBorder;

        [Header("For Debug")]
        [SerializeField] private GridNode[] ArrayGridNodes;

        [SerializeField] private int ActiveAnimationCount = 0;

        public override void Init()
        {
            base.Init();
            CalculateGridSizeAndPosition();
            CreateGrid();
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

            FallStartPoint.sizeDelta = GridNodesParent.sizeDelta;
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

                    if (gridSO.layers[0].gridItemsCreateType[index] == NodeItemCreateId.SPACE)
                        continue;

                    ArrayGridNodes[index] = Instantiate(GridNodePrefab, GridNodesParent);
                    ArrayGridNodes[index].name = $"Node[{x},{y}]";
                    ArrayGridNodes[index].x = x;
                    ArrayGridNodes[index].y = y;    
                    ArrayGridNodes[index].OnClick = OnClickGridNode;
                    ArrayGridNodes[index].RectTransform.anchoredPosition = new Vector2(x * spaceX, y * spaceY);

                    //SettingBorder
                    DynamicGridBorder.SetFilledArea(x, y);

                    //Creating Nodes' Items
                    for (int layer = 0; layer < gridSO.layers.Length; layer++)
                    {
                        NodeItemIds itemId = Utilities.CreateTypeToItemType(gridSO.layers[layer].gridItemsCreateType[index]);

                        if (itemId == NodeItemIds.NONE)
                            continue;

                        NodeItemBase itemPrefab = GridNodeItemPrefabsSO.Instance.GetGridItemPrefab(itemId);
                        NodeItemBase gridItem = OPNodeItem.Instantiate(itemPrefab, GridNodesItemParent);
                        gridItem.OnExploded = OnExplodedNodeItem;
                        gridItem.OnTriggered = OnTriggeredNodeItem;

                        if (layer == 0)
                        {
                            ArrayGridNodes[index].DynamicItem = (NodeDynamicItem)gridItem;
                            ArrayGridNodes[index].DynamicItem.State = MainItemStates.GROUND;
                        }
                        else
                            ArrayGridNodes[index].Items.Add(gridItem);


                        gridItem.RectTransform.anchoredPosition = ArrayGridNodes[index].RectTransform.anchoredPosition;
                    }
                }
            }

            DynamicGridBorder.CreateBorder();
        }

        #region Events

        private void OnTriggeredNodeItem(NodeItemBase item)
        {
            if (item.ItemType == NodeItemTypes.CUBE)
            {
                StartCoroutine(ITriggerCube(item.Node));
            }
            else if (item.ItemType == NodeItemTypes.ROCKET)
            {
                StartCoroutine(ITriggerRocket(item.Node, item));
            }
            else if(item.ItemType == NodeItemTypes.DUCK)
            {
                item.Explode( ExplodeConditions.BOTTOM_ROW);
                FillColumn(item.Node.x);
                FallColumn(item.Node.x);
            }
        }

        private void OnExplodedNodeItem(NodeItemBase item, ExplodeConditions condition)
        {
            if (item.ItemType == NodeItemTypes.DUCK)
            {
                FillColumn(item.Node.x);
                FallColumn(item.Node.x);
            }

        }

        private void OnClickGridNode(GridNode clickGridNode)
        {
            Metelab.Log(this, $"{clickGridNode.name} : Neighbour Nodes Count: {GetNodeNeighbours(clickGridNode).Length}" );

            if(ActiveAnimationCount  == 0)
            {
                if (clickGridNode.DynamicItem.TriggerCondition.HasFlag(TriggerConditions.CLICK))
                {
                    clickGridNode.DynamicItem.Trigger();
                }
                else if (clickGridNode.DynamicItem.ExplodeCondition.HasFlag(ExplodeConditions.CLICK))
                {
                    clickGridNode.DynamicItem.Explode(ExplodeConditions.CLICK);
                }
            }
        }
        #endregion

        #region Triggers

        private IEnumerator ITriggerRocket(GridNode node,NodeItemBase rocket)
        {
            GridNode[] explodeNodes = null;
            ActiveAnimationCount++;

            //WARNING: if Node size can be asymmetric, we need to get rocket type. Is it horizontal or vertical?
            float stepTime = GridNodePrefab.RectTransform.sizeDelta.x / (Constants.ROCKET_SPEED );

            int rightStartIndex,leftStartIndex;

            if (rocket.ItemId == NodeItemIds.ROCKET_HORIZONTAL)
            {
                explodeNodes = GetRow(node.y);
                rightStartIndex = node.x + 1;
                leftStartIndex = node.x - 1;
            }
            else // NodeItemIds.ROCKET_VERTICAL
            {
                explodeNodes = GetColumn(node.x);
                rightStartIndex = node.y + 1;
                leftStartIndex = node.y - 1;
            }


            for (int i = leftStartIndex, j = rightStartIndex; 0 <= i || j < explodeNodes.Length; i--, j++)
            {
                if (0 <= i && explodeNodes[i].IsHaveDynamicItem)
                {
                    explodeNodes[i].DynamicItem.Explode(ExplodeConditions.ROCKET);
                }
                    
                if (j < explodeNodes.Length && explodeNodes[j].IsHaveDynamicItem)
                {
                    explodeNodes[j].DynamicItem.Explode(ExplodeConditions.ROCKET);
                }
                 

                yield return new WaitForSeconds(stepTime);
            }

            ActiveAnimationCount--;

            if (ActiveAnimationCount == 0)
                FillAndFall();
        }

        private IEnumerator ITriggerCube(GridNode node)
        {
            GridNode[] matchNodes = GetMatch(node);

            if (matchNodes.Length > 1)
            {
                for (int i = 0; i < matchNodes.Length; i++)
                {
                    matchNodes[i].DynamicItem.Explode(ExplodeConditions.MATCH);
                }

                GridNode[] explodeSideNodes = GetNodesNeighbours(matchNodes);

                for (int i = 0; i < explodeSideNodes.Length; i++)
                {
                    for (int j = 0; j < explodeSideNodes[i].Items.Count; j++)
                    {
                        if (explodeSideNodes[i].IsHaveDynamicItem)
                        {
                            explodeSideNodes[i].Items[j].Explode(ExplodeConditions.MATCH_SIDE);
                        }
                    }
                }

                yield return new WaitForSeconds(0.1f);
                FillAndFall();
            }
        }


        private void FillAndFall()
        {
            GridNode[] emptyNodes = FindAllEmptyGridNodes();

            if (emptyNodes != null)
            {
                int[] rowIndexs = GetRowIndexs(emptyNodes);

                FillGrid(rowIndexs);
                FallGrid(rowIndexs);
            }
        }

        private void FillGrid(int[] colomns)
        {
            foreach (var column in colomns)
            {
                FillColumn(column);
            }
        }

        private void FillColumn(int column)
        {
            for (int y = 1; y < gridSO.height; y++)
            {
                GridNode currentNode = ArrayGridNodes[column + (y * gridSO.width)];

                if (currentNode == null || !currentNode.IsHaveDynamicItem) // for pass space or empty nodes
                    continue;

                GridNode firstEmptyNode = FindFirstEmptyNodeFromBot(currentNode);

                if (firstEmptyNode != null)
                {
                    NodeDynamicItem dynamicItem = currentNode.TakeDynamicItem();
                    firstEmptyNode.DynamicItem = dynamicItem;
                    dynamicItem.State = MainItemStates.FILL;
                }
            }
        }

        private void FallGrid(int[] colomns)
        {
            foreach(var column in colomns)
            {
                FallColumn(column);
            }
        }

        private void FallColumn(int column)
        {
            float totalHeight = 0;

            for (int y = 0; y < gridSO.height; y++)
            {
                GridNode emptyNode = ArrayGridNodes[column + y * gridSO.width];

                if (emptyNode!= null && !emptyNode.IsHaveDynamicItem)
                {
                    //Creating New Cube
                    NodeItemIds itemId = Utilities.CreateTypeToItemType(NodeItemCreateId.CUBE_RANDOM);
                    NodeItemBase itemPrefab = GridNodeItemPrefabsSO.Instance.GetGridItemPrefab(itemId);
                    NodeDynamicItem newItem = (NodeDynamicItem)Instantiate(itemPrefab, FallStartPoint);
                    totalHeight += newItem.RectTransform.sizeDelta.y + 20;
                    newItem.OnExploded = OnExplodedNodeItem;
                    newItem.OnTriggered = OnTriggeredNodeItem;
                    newItem.RectTransform.anchoredPosition = new Vector2(emptyNode.RectTransform.anchoredPosition.x, totalHeight);
                    newItem.RectTransform.SetParent(GridNodesItemParent);
                    newItem.State = MainItemStates.FALL;
                    emptyNode.DynamicItem = newItem;
                }
            }
        }

        #endregion

        #region GridOperations

        public GridNode[] GetRow(int y)
        {
            GridNode[] row = new GridNode[gridSO.width];

            for (int x = 0; x < gridSO.width; x++)
            {
                row[x] = ArrayGridNodes[x + y * gridSO.width];
            }

            return row;
        }

        public GridNode[] GetColumn(int x)
        {
            GridNode[] column = new GridNode[gridSO.height];

            for (int y = 0; y < gridSO.height; y++)
            {
                column[y] = ArrayGridNodes[x + y * gridSO.width];
            }

            return column;
        }


        private GridNode[] GetMatch(GridNode node)
        {
            List<GridNode> effectedNodes = new List<GridNode>();
            effectedNodes.Add(node);

            List<GridNode> lastFoundNodes = new List<GridNode>(GetSameItemSideNeighbourNodes(node));
            effectedNodes.AddRange(lastFoundNodes);

            List<GridNode> currentFoundNodes = new List<GridNode>();


            while (lastFoundNodes.Count > 0)
            {
                for (int i = 0; i < lastFoundNodes.Count; i++)
                {
                    currentFoundNodes = currentFoundNodes.Union(GetSameItemSideNeighbourNodes(lastFoundNodes[i])).ToList();
                }

                //string log = "";
                //foreach (var item in currentFoundNodes)
                //{
                //    log += $"{item.name}, ";
                //}
                //Metelab.Log(log,Color.yellow);

                currentFoundNodes = currentFoundNodes.Except(effectedNodes).ToList();
                effectedNodes.AddRange(currentFoundNodes);
                lastFoundNodes = new List<GridNode>(currentFoundNodes);
                
                currentFoundNodes.Clear();
            }

            return effectedNodes.ToArray();
        }


        /// <summary>
        /// It gets node which have same itemType from side neighbours
        /// </summary>
        private GridNode[] GetSameItemSideNeighbourNodes(GridNode node)
        {
            GridNode[] neighbourNode = GetNodeNeighbours(node);
            List<GridNode> sameItemNodes = new List<GridNode>();


            foreach (GridNode neighbour in neighbourNode)
            {
                if (neighbour.IsActive && neighbour.DynamicItem.ItemId == node.DynamicItem.ItemId)
                    sameItemNodes.Add(neighbour);
            }

            return sameItemNodes.ToArray();
        }

        /// <summary>
        /// It gets only up, right, down and left neighbour
        /// </summary>
        private GridNode[] GetNodeNeighbours(GridNode node)
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

        private GridNode[] GetNodesNeighbours(GridNode[] nodes)
        {
            List<GridNode> neighbourNodes = new List<GridNode>();

            for (int i = 0; i < nodes.Length; i++)
            {
                neighbourNodes = neighbourNodes.Union(GetNodeNeighbours(nodes[i])).ToList();
            }

            return neighbourNodes.Except(nodes).ToArray();
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
            List<GridNode> emptyGridNodes = new List<GridNode>();

            for (int i = 0; i < ArrayGridNodes.Length; i++)
            {
                if(ArrayGridNodes[i] != null && !ArrayGridNodes[i].IsHaveDynamicItem)
                {
                    emptyGridNodes.Add(ArrayGridNodes[i]);
                }
            }

            return emptyGridNodes.ToArray();
        }


        private GridNode FindFirstEmptyNodeFromBot(GridNode node)
        {
            GridNode current = null;
            for (int y = 0; y < node.y; y++)
            {
                current = ArrayGridNodes[node.x + (y * gridSO.width)];

                if (current != null && !current.IsHaveDynamicItem)
                    return current;
            }

            return null;
        }

        private int[] GetRowIndexs(GridNode[] gridNodes)
        {
            List<int> toFillColumns = new List<int>();

            foreach (var node in gridNodes)
            {
                if (!toFillColumns.Contains(node.x))
                    toFillColumns.Add(node.x);
            }

            return toFillColumns.ToArray();
        }

        #endregion
    }


}

