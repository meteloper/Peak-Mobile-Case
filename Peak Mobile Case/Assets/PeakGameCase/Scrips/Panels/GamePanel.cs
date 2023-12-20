using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GamePanel : MetePanel
    {
        private static MeteObjectPool<NodeItem> OPNodeItem = new MeteObjectPool<NodeItem>();

        [Header("GamePanel")]
        public GridSO gridSO;
        public RectTransform FallStartPoint;
        public RectTransform GridNodesParent;
        public RectTransform GridNodesItemParent;
        public GridNode GridNodePrefab;
        public DynamicGridBorder DynamicGridBorder;
        public GoalsBoard GoalsBoard;

        [Header("For Debug")]
        [SerializeField] private GridNode[] ArrayGridNodes;

        [SerializeField] private int ActiveAnimationCount = 0;

        public override void Init()
        {
            base.Init();
            CalculateGridSizeAndPosition();
            CreateGrid();
            GoalsBoard.SetGoals(gridSO.Goals);
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

                    if (gridSO.layers[0].gridItemsCreateType[index] == NodeItemCreateIds.SPACE)
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

                        NodeItem nodeItem = CreateNodeItem(ArrayGridNodes[index],itemId);

                    }
                }
            }

            DynamicGridBorder.CreateBorder();
        }

        private NodeItem CreateNodeItem(GridNode node, NodeItemIds itemId, Transform parent = null)
        {
            NodeItem itemPrefab = NodeItemPrefabsSO.Instance.GetGridItemPrefab(itemId);
            NodeItem nodeItem = OPNodeItem.Instantiate(itemPrefab, parent != null ? parent : GridNodesItemParent);
            nodeItem.OnExploded = OnExplodedNodeItem;
            nodeItem.OnTriggered = OnTriggeredNodeItem;
            nodeItem.RectTransform.anchoredPosition = node.RectTransform.anchoredPosition;
            node.AddItem(nodeItem);
            return nodeItem;
        }

        #region Events

        private void OnTriggeredNodeItem(NodeItem item)
        {
            if (item.ItemType == NodeItemTypes.CUBE)
            {
                StartCoroutine(ITriggerCube(item.GridNode));
            }
            else if (item.ItemType == NodeItemTypes.ROCKET)
            {
                StartCoroutine(ITriggerRocket(item.GridNode, item));
            }
        }

        private void OnExplodedNodeItem(NodeItem item, ExplodeConditions condition)
        {
            if (item.ItemType == NodeItemTypes.TOY)
            {
                FillColumn(item.GridNode.x);
                FallColumn(item.GridNode.x);
            }
        }

        private void OnClickGridNode(GridNode clickGridNode)
        {
            Metelab.Log(this, clickGridNode.name);

            if (ActiveAnimationCount  == 0)
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

        private IEnumerator ITriggerRocket(GridNode node,NodeItem rocket)
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
                if (0 <= i && explodeNodes[i]!= null && explodeNodes[i].IsHaveDynamicItem)
                {
                    explodeNodes[i].DynamicItem.Explode(ExplodeConditions.ROCKET);
                }
                    
                if (j < explodeNodes.Length && explodeNodes[j] != null && explodeNodes[j].IsHaveDynamicItem)
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
            GridNode[] explodeSideNodes = GetNodesNeighbours(matchNodes);
            Metelab.Log(this, $"{nameof(ITriggerCube)}: {node.name} : Match Count: {matchNodes.Length}");

            if (matchNodes.Length >= 5)
            {
                ExplodeNodes(matchNodes, ExplodeConditions.MERGE);
                StartCoroutine(IMergeCube(matchNodes, Utilities.CreateTypeToItemType(NodeItemCreateIds.RANDOM_ROCKET)));
                ExplodeNodes(explodeSideNodes, ExplodeConditions.MATCH_SIDE);
            }
            else if (matchNodes.Length > 1)
            {
                ExplodeNodes(matchNodes,ExplodeConditions.MATCH);
                ExplodeNodes(explodeSideNodes, ExplodeConditions.MATCH_SIDE);

                yield return new WaitForSeconds(0.1f);
                FillAndFall();
            }
        }

        private IEnumerator IMergeCube(GridNode[] nodes,NodeItemIds itemID)
        {
            ActiveAnimationCount++;
            NodeItem[] nodesItem = new NodeItem[nodes.Length];

            nodesItem[0] = nodes[0].TakeDynamicItem();
            nodesItem[0].transform.SetParent(GridNodesItemParent.parent);
            ((Cube)nodesItem[0]).MergeLight.SetActive(true);

            float totalTime = 0.36f;
            float scale = 1.25f;

            for (int i = 1; i < nodes.Length; i++)
            {
                nodesItem[i] = nodes[i].TakeDynamicItem();
                nodesItem[i].transform.SetParent(nodesItem[0].transform);
                nodesItem[i].NodeItemDynamic?.Cube?.MergeLight.SetActive(true);
                nodesItem[i].RectTransform.DOAnchorPos(nodesItem[i].RectTransform.anchoredPosition * scale, totalTime * 0.33f).SetEase(Ease.OutQuad);
            }

            yield return new WaitForSeconds(totalTime / 3);

            for (int i = 1; i < nodes.Length; i++)
            {
                nodesItem[i].RectTransform.DOAnchorPos(Vector2.zero, totalTime * 0.66f).SetEase(Ease.InQuad);
            }

            yield return new WaitForSeconds(totalTime * 0.72f);


            for (int i = 0; i < nodes.Length; i++)
            {
                nodesItem[i].transform.SetParent(GridNodesItemParent);
                nodesItem[i].NodeItemDynamic?.Cube?.MergeLight.SetActive(false);
                nodesItem[i].gameObject.SetActive(false);
            }

            CreateNodeItem(nodes[0], itemID).NodeItemDynamic?.Rocket?.PlayMergeIntroEffects();

            

            yield return null;

            ActiveAnimationCount--;

            if(ActiveAnimationCount == 0)
                FillAndFall();
        }

        private void ExplodeNodes(GridNode[] nodes, ExplodeConditions condition)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < nodes[i].Items.Count; j++)
                {
                    nodes[i].Items[j].Explode(condition);
                }
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
                    NodeItemDynamic dynamicItem = currentNode.TakeDynamicItem();
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
                    NodeItemIds itemId = Utilities.CreateTypeToItemType(NodeItemCreateIds.RANDOM_CUBE);
                    NodeItemDynamic newItem = CreateNodeItem(emptyNode, itemId, FallStartPoint).NodeItemDynamic;
                    totalHeight += newItem.RectTransform.sizeDelta.y + 20;
                    newItem.RectTransform.anchoredPosition = new Vector2(emptyNode.RectTransform.anchoredPosition.x, totalHeight);
                    newItem.RectTransform.SetParent(GridNodesItemParent);
                    newItem.State = MainItemStates.FALL;
                }
            }
        }

        #endregion

        #region GridOperations

        public GridNode[] GetRow(int y)
        {
            GridNode[] row = new GridNode[gridSO.width];
            int index;

            for (int x = 0; x < gridSO.width; x++)
            {
                index = x + y * gridSO.width;
                row[x] = ArrayGridNodes[index];
            }

            return row;
        }

        public GridNode[] GetColumn(int x)
        {
            GridNode[] column = new GridNode[gridSO.height];
            int index;
            for (int y = 0; y < gridSO.height; y++)
            {
                index = x + y * gridSO.width;
                column[y] = ArrayGridNodes[index];
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

