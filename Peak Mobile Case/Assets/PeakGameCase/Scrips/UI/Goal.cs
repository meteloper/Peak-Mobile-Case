using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Metelab.PeakGameCase
{
    public class Goal: MeteMono
    {
        public Image ImageIcon;
        public TextMeshProUGUI TextCount;

        [SerializeField] private int countdown;
        public int Countdown
        {
            get
            {
                return countdown;
            }
            set
            {
                if(value >= 0)
                {
                    countdown = value;
                    TextCount.text = value.ToString();
                }
            }
        }

        private GoalSO goalSO;

        public void StartGoal(GoalSO goalSO)
        {
            this.goalSO = goalSO;
            Countdown = goalSO.Count;
            GameEvents.OnExplodedNodeItem += OnExplodedNodeItem;
            ImageIcon.sprite = goalSO.GoalIcon;
        }

        private void OnDestroy()
        {
            GameEvents.OnExplodedNodeItem -= OnExplodedNodeItem;
        }

        public void TickCountdown()
        {
            Countdown--;
        }


        #region Events
        private void OnExplodedNodeItem(NodeItem nodeItem)
        {
            Metelab.Log(this, nodeItem.ItemId.ToString());
            foreach (var requirement in goalSO.GoalItemsRequirements)
            {
                if(requirement.NodeItemID == NodeItemIds.NONE )
                {
                    if (requirement.NodeItemType == nodeItem.ItemType)
                    {
                        TickCountdown();
                    }
                }
                else
                {
                    if (requirement.NodeItemID == nodeItem.ItemId)
                    {
                        TickCountdown();
                    }
                }
            }
        }

        #endregion


    }
}
