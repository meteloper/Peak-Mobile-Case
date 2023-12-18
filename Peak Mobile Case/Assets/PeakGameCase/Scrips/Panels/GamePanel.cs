using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.Assertions.Must;
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
        [SerializeField] private GameStates currentGameState;

        public override void Init()
        {
            base.Init();
            CalculateGridSizeAndPosition();
            CreateGrid();
            currentGameState = GameStates.CAN_MOVE;
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
            Metelab.Log(this, item.ItemId.ToString());

            if(item.ExplodeCondition == ExplodeConditions.BOTTOM_ROW)
            {
                FillColumn(item.Node.x);
                FallColumn(item.Node.x);

                if(item.ItemType == NodeItemTypes.DUCK)
                {
                    AudioManager.Instance.PlayOneShot(AudioNames.Duck);
                }
            }
        }

        private void OnClickGridNode(GridNode clickGridNode)
        {
            Metelab.Log(this, $"{clickGridNode.name} : Neighbour Nodes Count: {GetNodeNeighbours(clickGridNode).Length}" );

            if (clickGridNode.Items.Count > 0 && clickGridNode.Items[0] != null && clickGridNode.Items[0].ItemType == NodeItemTypes.CUBE)
            {
                GridNode[] matchNodes = FindMatch(clickGridNode);

                if(matchNodes.Length > 1)
                {
                    StartCoroutine(IExplodeMatch(matchNodes));
                }
            }
        }
        #endregion

        #region States
        private IEnumerator IExplodeMatch(GridNode[] explodeNodes)
        {
            AudioManager.Instance.PlayOneShot(AudioNames.CubeExplode);

            for (int i = 0; i < explodeNodes.Length; i++)
            {
                explodeNodes[i].Items[0].Explode(ExplodeConditions.EXPLODE);
            }

            GridNode[] explodeSideNodes = GetNodesNeighbours(explodeNodes);

            bool isHaveBalloon = false;

            for (int i = 0; i < explodeSideNodes.Length; i++)
            {
                for (int j = 0; j < explodeSideNodes[i].Items.Count; j++)
                {
                    if (explodeSideNodes[i].Items[j] != null)
                    {
                        if (explodeSideNodes[i].Items[j].ItemType == NodeItemTypes.BALLOON)
                            isHaveBalloon = true;

                        explodeSideNodes[i].Items[j].Explode(ExplodeConditions.EXPLODE_SIDE);
                    }
                }
            }

            AudioManager.Instance.PlayOneShot(AudioNames.CubeExplode);

            if (isHaveBalloon)
                AudioManager.Instance.PlayOneShot(AudioNames.Balloon);

            GridNode[] emptyNodes = FindAllEmptyGridNodes();
            if (emptyNodes != null)
            {
                List<int> toFillColumns = new List<int>();

                for (int i = 0; i < emptyNodes.Length; i++)
                {
                    if (!toFillColumns.Contains(emptyNodes[i].x))
                        toFillColumns.Add(emptyNodes[i].x);
                }

                yield return new WaitForSeconds(0.1f);
                FillGrid(toFillColumns.ToArray());
                FallGrid(toFillColumns.ToArray());
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
        private GridNode[] FindMatch(GridNode node)
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

        

        private YieldInstruction MoveMainItemToNodeWithAcceleration(RectTransform moveItem,RectTransform target, Ease ease, float acceleration, Action OnCompleted = null)
        {
            float targetY = target.anchoredPosition.y;
            float currentY = moveItem.anchoredPosition.y;
            float totalTime = Mathf.Pow(2 * Math.Abs(currentY - targetY) / acceleration, 0.5f); // t =  ((2*d)/a)^(1/2)

            return moveItem.
              DOAnchorPosY(targetY, totalTime).
              SetEase(ease).
              OnComplete(() => { OnCompleted?.Invoke(); }).
              WaitForCompletion();
        }

        private YieldInstruction MoveMainItemToNodeWithTime(RectTransform moveItem, RectTransform target, Ease ease, float time, Action OnCompleted = null)
        {
            return moveItem.
              DOAnchorPosY(target.anchoredPosition.y, time).
              SetEase(ease).
              OnComplete(() => { OnCompleted?.Invoke(); }).
              WaitForCompletion();
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

        private void TransferFirstLayerItem(GridNode from, GridNode to)
        {
            if (to.Items != null)
            {
                to.Items = new List<NodeItemBase>() { from.Items[0] };
                from.Items[0] = null;
            }
            else if (to.Items.Count == 0)
            {
                to.Items.Add(from.Items[0]);
                from.Items[0] = null;
            }
        }

        #endregion
    }


}

