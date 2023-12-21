using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    [CreateAssetMenu(fileName = "GoalItemPrefabs", menuName = "PeakGameCase/Single/GoalItemPrefabs")]
    public class GoalItemPrefabsSO : MeteSingletonScriptableObject<GoalItemPrefabsSO>
    {
        public GoalItem[] GoalItems;

        public GoalItem this[GoalItemIds itemId]
        {
            get { return GoalItems[(int)itemId]; }
        }





    }
}
