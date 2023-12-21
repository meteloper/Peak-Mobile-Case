using System;

namespace Metelab.PeakGameCase
{
    public static class GameEvents
    {
        public static event Action<NodeItem[], ExplodeConditions> BeforeExplodeNodeItems;

        public static void InvokeBeforeExplodeGridNodes(NodeItem[] items, ExplodeConditions explodeCondition)
        {
            BeforeExplodeNodeItems?.Invoke(items, explodeCondition);
        }


        public static event Action<NodeItem[], ExplodeConditions> AfterExplodeNodeItems;

        public static void InvokeAfterExplodeNodeItems(NodeItem[] items, ExplodeConditions explodeCondition)
        {
            AfterExplodeNodeItems?.Invoke(items, explodeCondition);
        }


        public static event Action<NodeItem> OnNodeItemGround;

        public static void InvokeOnNodeItemGround(NodeItem item)
        {
            OnNodeItemGround?.Invoke(item);
        }

        public static event Action OnStartedMove;

        public static void InvokeOnStartedMove()
        {
            OnStartedMove?.Invoke();
        }

        public static event Action<EndGameResult> OnGameOver;
        public static void InvokeOnGameOver(EndGameResult result)
        {
            OnGameOver?.Invoke(result);
        }
    }
}
