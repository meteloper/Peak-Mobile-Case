using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GoalsBoard : MeteMono
    {
        MeteObjectPool<Goal> OP_PrefabGoal = new MeteObjectPool<Goal>();

        public RectTransform GoalsParent;
        public Goal PrefabGoal;

        public void SetGoals(GoalSO[] goals)
        {
            GoalsParent.SetActiveChildren(false);

            foreach (GoalSO goal in goals)
            {
                 OP_PrefabGoal.Instantiate(PrefabGoal, GoalsParent).StartGoal(goal);
            }
        }

    }
}
