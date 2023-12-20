

using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GoalItem : MeteMono
    {
        public virtual void Play( Goal targetGoal)
        {
            RectTransform.SetParent(targetGoal.RectTransform);
            RectTransform.DOAnchorPos(Vector3.zero, 0.5f).OnComplete(() => { targetGoal.TickCountdown(); });
        }
    }
}
