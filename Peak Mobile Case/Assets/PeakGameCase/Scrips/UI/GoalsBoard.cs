using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GoalsBoard : MeteMono
    {
        private MeteObjectPool<Goal> OP_PrefabGoal = new MeteObjectPool<Goal>();
        private MeteObjectPool<GoalItem> OP_PrefabGoalItems = new MeteObjectPool<GoalItem>();

        public RectTransform GoalsParent;
        public Goal PrefabGoal;
        private Goal[] goals;
        private bool isGameOver = true;

        public void SetGoals(GoalSO[] goalsSO)
        {
            isGameOver = false;
            OP_PrefabGoal.DeactivePool();
            OP_PrefabGoalItems.DeactivePool();

            goals = new Goal[goalsSO.Length];

            for (int i = 0; i < goalsSO.Length; i++)
            {
                goals[i] = OP_PrefabGoal.Instantiate(PrefabGoal, GoalsParent);
                goals[i].StartGoal(goalsSO[i]);
            }
        }

        private void Start()
        {
            GameEvents.BeforeExplodeNodeItems += BeforeExplodedNodeItems;
        }

        private void OnDestroy()
        {
            GameEvents.BeforeExplodeNodeItems -= BeforeExplodedNodeItems;
        }


        private Goal WhichGoalIsNeed(GoalItemIds goalItemId )
        {
            for (int i = 0; i < goals.Length; i++)
            {
                if(goals[i].RealtimeCount > 0)
                {
                    foreach (var item in goals[i].GoalSO.GoalItemIds)
                    {
                        if(item == goalItemId)
                        {
                            return goals[i];
                        }
                    }
                }
            }

            return null;
        }

        public void StartGameOverControl()
        {
            if (isGameOver)
                return;
            
            StartCoroutine(IGameOverControl());
        }

   
        IEnumerator IGameOverControl()
        {
            isGameOver = true;

            for (int i = 0; i < goals.Length; i++)
            {
                if (goals[i].RealtimeCount > 0)
                {
                    isGameOver = false;
                }
            }

            if(isGameOver)
            {
                yield return new WaitForSeconds(1);
                GameEvents.InvokeOnGameOver(EndGameResult.WIN);
            }
        }



        #region Events
        private void BeforeExplodedNodeItems(NodeItem[] nodeItems,ExplodeConditions explodeCondition)
        {

            GoalItemIds goalItemId = nodeItems[0].GoalItemId;
            Goal targetGoal = WhichGoalIsNeed(goalItemId);

            if (targetGoal != null)
            {
                if (!explodeCondition.HasFlag(ExplodeConditions.MERGE))
                {
                    GoalItem goalItemPrefab = GoalItemPrefabsSO.Instance[goalItemId];

                    foreach (var item in nodeItems)
                    {
                        if(targetGoal.RealtimeCount > 0)
                        {
                            targetGoal.RealtimeCount--;
                            GoalItem newGoalItem = OP_PrefabGoalItems.Instantiate(goalItemPrefab, item.Center.position, Quaternion.identity, targetGoal.GoalItemParents);
                            newGoalItem.Play(targetGoal.RectTransform, 0).OnComplete(() => { targetGoal.Countdown--; newGoalItem.Deactive(); });
                        }
                    }
                }
                else
                {
                    targetGoal.RealtimeCount-= nodeItems.Length;
                    targetGoal.Countdown -= nodeItems.Length;
                }

                StartGameOverControl();
            }

        

        }



        #endregion

    }
}
