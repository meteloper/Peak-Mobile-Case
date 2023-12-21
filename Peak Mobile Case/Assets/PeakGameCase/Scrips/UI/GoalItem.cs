

using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class GoalItem : MeteMono
    {
        public Canvas layerCanvas;

        public virtual Tween Play( RectTransform target,float delay)
        {
            layerCanvas.sortingOrder = 1;

            DOVirtual.DelayedCall(0.5f, () => { layerCanvas.sortingOrder = 10; });
            RectTransform.DOScale(0.7f, 0.2f).SetDelay(delay);
            RectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuad).SetDelay(delay);
     
            return RectTransform.DOAnchorPosY(0, 1).SetEase(Ease.InBack).SetDelay(delay);
        }
    }
}
