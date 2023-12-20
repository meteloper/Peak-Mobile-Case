using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    [CreateAssetMenu(fileName = "GoalItemPrefabs", menuName = "PeakGameCase/Single/GoalItemPrefabs")]
    public class GoalItemPrefabsSO : ScriptableSingleton<GoalItemPrefabsSO>
    {
        public GoalItem[] GoalItems;



    }
}
