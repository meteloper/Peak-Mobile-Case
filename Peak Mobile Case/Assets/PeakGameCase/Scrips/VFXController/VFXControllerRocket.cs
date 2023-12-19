using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class VFXControllerRocket : VFXControllerBase
    {
        public RectTransform RightPart;
        public RectTransform LeftPart;

        public bool isVertical;

        public override void Play()
        {
            RightPart.anchoredPosition = Vector2.zero;
            LeftPart.anchoredPosition = Vector2.zero;

            if (isVertical)
            {
                RightPart.eulerAngles = new Vector3(0, 0, 90);
                LeftPart.eulerAngles = new Vector3(0, 0, 90);

                float time = Screen.height / Constants.ROCKET_SPEED;
                RightPart.DOAnchorPosY(Screen.height, time).SetEase( Ease.Linear);
                LeftPart.DOAnchorPosY(-Screen.height, time).SetEase(Ease.Linear);
            }
            else //horizontal
            {
                RightPart.eulerAngles = Vector3.zero;
                LeftPart.eulerAngles = Vector3.zero;

                float time = Screen.width / Constants.ROCKET_SPEED;
                RightPart.DOAnchorPosX(Screen.width, time).SetEase(Ease.Linear);
                LeftPart.DOAnchorPosX(-Screen.width, time).SetEase(Ease.Linear);
            }

            base.Play();

        }
    }

}
