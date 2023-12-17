using DG.Tweening;
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
                    ArrayGridNodes[index].OnClick += OnClickGridNode;
                    ArrayGridNodes[index].RectTransform.anchoredPosition = new Vector2(x * spaceX, y * spaceY);

                    //SettingBorder
                    DynamicGridBorder.SetFilledArea(x, y);

                    //Creating Nodes' Items
                    for (int i = 0; i < gridSO.layers.Length; i++)
                    {
                        NodeItemId itemType = Utilities.CreateTypeToItemType(gridSO.layers[i].gridItemsCreateType[index]);
                        Metelab.Log("("+x+","+y+")  "+i+" "+ gridSO.layers[i].gridItemsCreateType[index].ToString()+" "+ itemType);

                        if (itemType == NodeItemId.NONE)
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

        #region Events
        private void OnClickGridNode(GridNode clickGridNode)
        {
            Metelab.Log(this, $"{clickGridNode.name} : Neighbour Nodes Count: {GetNodeNeighbours(clickGridNode).Length}" );

            if(currentGameState == GameStates.CAN_MOVE)
            {
                if (clickGridNode.Items.Count > 0 && clickGridNode.Items[0] != null && clickGridNode.Items[0].ItemType == NodeItemType.CUBE)
                {
                    GridNode[] matchNodes = FindMatch(clickGridNode);

                    if(matchNodes.Length > 1)
                    {
                        currentGameState = GameStates.MOVE_STARTED;
                       
                        StartCoroutine(IExplodeMatch(matchNodes));
                    }
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

                string log = "";
                foreach (var item in currentFoundNodes)
                {
                    log += $"{item.name}, ";
                }
                Metelab.Log(log,Color.yellow);

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
                if (neighbour.Items[0] != null && neighbour.Items[0].ItemId == node.Items[0].ItemId)
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
                if(ArrayGridNodes[i] != null && !ArrayGridNodes[i].IsHaveItemInFirstLayer)
                {
                    emptyGridNodes.Add(ArrayGridNodes[i]);
                }
            }

            return emptyGridNodes.ToArray();
        }

        private IEnumerator IExplodeMatch(GridNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].Items[0].Explode();
                nodes[i].Items[0] = null;
            }

            AudioManager.Instance.PlayOneShot(AudioNames.CubeExplode);

            GridNode[] toTriggerNodes = GetNodesNeighbours(nodes);

            for (int i = 0; i < toTriggerNodes.Length; i++)
            {
                
                    for (int j = 0; j < toTriggerNodes[i].Items.Count; j++)
                    {
                        if (toTriggerNodes[i].Items[j] != null)
                            toTriggerNodes[i].Items[j].Trigger();
                    }
                
            }

            yield return new WaitForSeconds(0.1f);
            StartCoroutine(IFillGrid());
        }

        private IEnumerator IFillGrid()
        {

            GridNode[] emptyNodes = FindAllEmptyGridNodes();

            if(emptyNodes != null)
            {
                List<int> toFillColumns = new List<int>();

                for (int i = 0; i < emptyNodes.Length; i++)
                {
                    if (!toFillColumns.Contains(emptyNodes[i].x))
                        toFillColumns.Add(emptyNodes[i].x);
                }

                List<YieldInstruction> fillAnimastions = new List<YieldInstruction>();

                foreach (var column in toFillColumns)
                {
                    fillAnimastions.AddRange(FillColumn(column));
                }

                foreach (var anim in fillAnimastions)
                {
                    yield return anim;
                }
            }

            currentGameState = GameStates.CAN_MOVE;
        }

        private YieldInstruction[] FillColumn(int column)
        {
            GridNode current = null;
            GridNode firstEmptyPlace = null;
            List<YieldInstruction> animations = new List<YieldInstruction> ();

            for (int y = 1; y < gridSO.height; y++)
            {
                current = ArrayGridNodes[column + (y * gridSO.width)];

                if (current == null || !current.IsHaveItemInFirstLayer) // for pass space or empty nodes
                    continue;

                firstEmptyPlace = FindFirstEmptyNodeFromBot(current);

                if(firstEmptyPlace != null)
                {
                    TransferFirstLayerItem(current, firstEmptyPlace);

                    animations.Add(firstEmptyPlace.Items[0].RectTransform.
                        DOAnchorPosY(firstEmptyPlace.RectTransform.anchoredPosition.y, Constants.FILL_ANIM_TIME_SEC).
                        SetEase( Ease.InQuad).
                        WaitForCompletion());
                }
            }

            return animations.ToArray();
        }

        //
        private GridNode FindFirstEmptyNodeFromBot(GridNode node)
        {
            GridNode current = null;
            for (int y = 0; y < node.y; y++)
            {
                current = ArrayGridNodes[node.x + (y * gridSO.width)];

                if (current != null && !current.IsHaveItemInFirstLayer)
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

        private IEnumerator FallAnimations(GridNode[] gridNodesToFill)
        {


            yield return null;
        }

        #endregion
    }


}

