using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Metelab.PeakGameCase
{
    [CreateAssetMenu(fileName = "New Goal", menuName = "PeakGameCase/Goal")]
    public class GoalSO : MeteScriptableObject
    {
        public Sprite GoalIcon;
        public int Count;
        public GoalItemRequirements[] GoalItemsRequirements;
    }

    [Serializable]
    public class GoalItemRequirements
    {
        public GoalItemId GoalItemID;

        [Header("Requirements")]
        public NodeItemIds NodeItemID;
        public NodeItemTypes NodeItemType;
    }


}
