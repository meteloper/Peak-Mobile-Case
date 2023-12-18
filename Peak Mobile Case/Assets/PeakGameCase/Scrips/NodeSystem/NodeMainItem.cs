using Metelab.PeakGameCase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public abstract class NodeMainItem : NodeItemBase
    {
        public RectTransform Ground;
        private float fallSpeed = 0;
        public MainItemStates State;

        private void Update()
        {
            if(State == MainItemStates.FALL)
            {
                if (RectTransform.anchoredPosition.y - Ground.anchoredPosition.y > 0)
                {
                    fallSpeed += Time.deltaTime * Constants.FALL_ACCELERATION;
                    RectTransform.anchoredPosition = RectTransform.anchoredPosition + Vector2.down * fallSpeed * Time.deltaTime;

                    if (RectTransform.anchoredPosition.y - Ground.anchoredPosition.y <= 0)
                    {
                        RectTransform.anchoredPosition = Ground.anchoredPosition;
                        fallSpeed = 0;
                        State = MainItemStates.GROUND;
                    }
                }
            }
            else if(State == MainItemStates.FILL)
            {
                if (RectTransform.anchoredPosition.y - Ground.anchoredPosition.y > 0)
                {
                    RectTransform.anchoredPosition = RectTransform.anchoredPosition + Vector2.down * Constants.FILL_SPEED * Time.deltaTime;

                    if (RectTransform.anchoredPosition.y - Ground.anchoredPosition.y <= 0)
                    {
                        RectTransform.anchoredPosition = Ground.anchoredPosition;
                        State = MainItemStates.GROUND;
                    }
                }
            }
        }

        public abstract override void Explode();

        public abstract override void Trigger();

    }
}
