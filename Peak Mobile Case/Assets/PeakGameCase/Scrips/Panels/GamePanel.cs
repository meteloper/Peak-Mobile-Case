using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GamePanel : MetePanel
    {
        private static MeteObjectPool<NodeItem> OP_NodeItem = new MeteObjectPool<NodeItem>();
        private static MeteObjectPool<GridNode> OP_GridNode = new MeteObjectPool<GridNode>();

        [Header("GamePanel")]
        public GridSO gridSO;
        public RectTransform FallStartPoint;
        public RectTransform GridNodesParent;
        public RectTransform GridNodesItemParent;
        public GridNode GridNodePrefab;
        public DynamicGridBorder DynamicGridBorder;
        public GoalsBoard GoalsBoard;
        public MovesBoard MovesBoard;

        [Header("For Debug")]
        [SerializeField] private GridNode[] ArrayGridNodes;

        [SerializeField] private int ActiveAnimationCount = 0;

        public override void Init()
        {
            base.Init();
            GameEvents.AfterExplodeNodeItems += AfterExplodeNodeItems;
            GameEvents.OnNodeItemGround += OnNodeItemGround;
            GameEvents.OnGameOver += OnGameOver;
        }

        private void OnDestroy()
        {
            GameEvents.AfterExplodeNodeItems -= AfterExplodeNodeItems;
            GameEvents.OnNodeItemGround -= OnNodeItemGround;
            GameEvents.OnGameOver -= OnGameOver;
        }


        public void StartGame(GridSO newGridSO = null)
        {
            OP_NodeItem.DeactivePool();
            OP_GridNode.DeactivePool();

            if (newGridSO != null)
                this.gridSO = newGridSO;

            CalculateGridSizeAndPosition();
            CreateGrid();
            GoalsBoard.SetGoals(gridSO.Goals);
            MovesBoard.SetMoves(gridSO.maxMoveCount);
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

                    ArrayGridNodes[index] = OP_GridNode.Instantiate(GridNodePrefab, GridNodesParent);
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
            NodeItem itemPrefab = NodeItemPrefabsSO.Instance[itemId];
            NodeItem nodeItem = OP_NodeItem.Instantiate(itemPrefab, parent != null ? parent : GridNodesItemParent);
            nodeItem.RectTransform.anchoredPosition = node.RectTransform.anchoredPosition;
            node.AddItem(nodeItem);
            return nodeItem;
        }

        #region Events

        private void OnGameOver(EndGameResult result)
        {
            GameOverPanel gameOverPanel = PanelControllerMainScene.current.GetPanel<GameOverPanel>(PanelNames.GameOver);
            gameOverPanel.StartPanelForResult(result);
        }

       

        private void OnNodeItemGround(NodeItem item)
        {
            if (item.ExplodeCondition.HasFlag(ExplodeConditions.BOTTOM_ROW))
            {
                if (item.GridNode.y == FindBottomRomInColumn(item.GridNode.x))
                {
                    ExplodeNode(item.GridNode, ExplodeConditions.BOTTOM_ROW);

                    FillColumn(item.GridNode.x);
                    FallColumn(item.GridNode.x);
                }
            }
        }

        private void AfterExplodeNodeItems(NodeItem[] items,ExplodeConditions condition)
        {
            foreach (NodeItem item in items)
            {
                if (item.TriggerCondition.HasFlag(TriggerConditions.ON_EXPLODE))
                {
                    TriggerByItem(item);
                }
            }
        }

        private void OnClickGridNode(GridNode clickGridNode)
        {
            Metelab.Log(this, clickGridNode.name);

            if (ActiveAnimationCount  == 0)
            {
                if (clickGridNode.DynamicItem.ClickEffect == ClickEffects.EXPLODE)
                {
                    ExplodeNode(clickGridNode);
                    GameEvents.InvokeOnStartedMove();
                }
                else if(clickGridNode.DynamicItem.ClickEffect == ClickEffects.TRIGGER)
                {
                    TriggerByItem(clickGridNode.DynamicItem);
                }
                else
                {
                    clickGridNode.DynamicItem.PlayShake();
                }
            }
        }
        #endregion

        #region Triggers

        private void TriggerByItem(NodeItem item)
        {
            switch (item.ItemType)
            {
                case NodeItemTypes.CUBE:
                    StartCoroutine(ITriggerCube(item.GridNode));
                    break;
                case NodeItemTypes.ROCKET:
                    StartCoroutine(ITriggerRocket(item.GridNode, item));
                    break;
            }
        }

        private IEnumerator ITriggerRocket(GridNode node, NodeItem item)
        {
            GridNode[] explodeNodes = null;
            ActiveAnimationCount++;

            //WARNING: if Node size can be asymmetric, we need to get rocket type. Is it horizontal or vertical?
            float stepTime = GridNodePrefab.RectTransform.sizeDelta.x / (Constants.ROCKET_SPEED );

            int rightStartIndex,leftStartIndex;

            if (item.ItemId == NodeItemIds.ROCKET_HORIZONTAL)
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
                    ExplodeNode(explodeNodes[i], ExplodeConditions.ROCKET);
                }
                    
                if (j < explodeNodes.Length && explodeNodes[j] != null && explodeNodes[j].IsHaveDynamicItem)
                {
                    ExplodeNode(explodeNodes[j], ExplodeConditions.ROCKET);
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
                GameEvents.InvokeOnStartedMove();
                StartCoroutine(IMergeCubeAnimations(matchNodes, Utilities.CreateTypeToItemType(NodeItemCreateIds.RANDOM_ROCKET)));
                ExplodeNodes(matchNodes, ExplodeConditions.MERGE);
                ExplodeNodes(explodeSideNodes, ExplodeConditions.MERGE_SIDE);

            }
            else if (matchNodes.Length > 1)
            {
                GameEvents.InvokeOnStartedMove();
                ExplodeNodes(matchNodes,ExplodeConditions.MATCH);
                ExplodeNodes(explodeSideNodes, ExplodeConditions.MATCH_SIDE);

                yield return new WaitForSeconds(0.1f);
                FillAndFall();
            }
            else
            {
                node.DynamicItem.PlayShake();
            }
        }

        private IEnumerator IMergeCubeAnimations(GridNode[] nodes,NodeItemIds itemID)
        {
            ActiveAnimationCount++;

            NodeItem[] nodesItem = new NodeItem[nodes.Length];
            nodesItem[0] = nodes[0].DynamicItem;
            nodesItem[0].transform.SetParent(GridNodesItemParent.parent);
            ((Cube)nodesItem[0]).MergeLight.SetActive(true);

            float totalTime = 0.36f;
            float scale = 1.25f;

            for (int i = 1; i < nodes.Length; i++)
            {
                nodesItem[i] = nodes[i].DynamicItem;
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

            if (ActiveAnimationCount == 0)
                FillAndFall();
        }


        private void ExplodeNode(GridNode node, ExplodeConditions condition = ExplodeConditions.NONE)
        {
            List<NodeItem> explodeNodeItems = new List<NodeItem>();

            foreach (var item in node.Items)
            {
                if (condition == ExplodeConditions.NONE || item.IsCanExplode(condition))
                {
                    explodeNodeItems.Add(item);
                }
            }

            if (explodeNodeItems.Count == 0)
                return;

            GameEvents.InvokeBeforeExplodeGridNodes(explodeNodeItems.ToArray(), condition);

            foreach (NodeItem item in explodeNodeItems)
            {
                item.PlayExplode();
                item.Deactive();
            }

            if (node.DynamicItem.IsCanExplode(condition))
                node.TakeDynamicItem();

            GameEvents.InvokeAfterExplodeNodeItems(explodeNodeItems.ToArray(), condition);
        }

        private void ExplodeNodes(GridNode[] nodes, ExplodeConditions condition)
        {
            List<NodeItem> explodeNodeItems = new List<NodeItem>();

            foreach (var node in nodes)
            {
                foreach (var item in node.Items)
                {
                    if (item.IsCanExplode(condition))
                    {
                        explodeNodeItems.Add(item);
                    }
                }
            }

            if (explodeNodeItems.Count == 0)
                return;

            GameEvents.InvokeBeforeExplodeGridNodes(explodeNodeItems.ToArray(), condition);

            foreach (var item in explodeNodeItems)
            {
                if (!condition.HasFlag(ExplodeConditions.MERGE))
                {

                    item.PlayExplode();
                    item.Deactive();
                }
            }

            foreach (var node in nodes)
            {
                if (node.DynamicItem.IsCanExplode(condition))
                {
                        node.TakeDynamicItem();
                }
            }

            GameEvents.InvokeAfterExplodeNodeItems(explodeNodeItems.ToArray(), condition);
        }


        #endregion

        #region Fill and Fall

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

        private int FindBottomRomInColumn(int x)
        {
            GridNode[] colums = GetColumn(x);

            for (int y = 0; y < colums.Length; y++)
            {
                if (colums[y] != null)
                {
                    return y;
                }
            }

            return -1;
        }

        #endregion
    }


}

