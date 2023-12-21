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
        public GoalItemIds[] GoalItemIds;
    }


}
