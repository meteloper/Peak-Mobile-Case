using DG.Tweening;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    [RequireComponent(typeof(Animation))]
    public class NodeDynamicItem : NodeItemBase
    {
        public MainItemStates State;
        private Animation animCollision;
        private float fallSpeed = 0;

        private void Start()
        {
            animCollision = GetComponent<Animation>();
        }

        private void Update()
        {
            if(State == MainItemStates.FALL)
            {
                if (RectTransform.anchoredPosition.y - Node.RectTransform.anchoredPosition.y > 0)
                {
                    fallSpeed += Time.deltaTime * Constants.FALL_ACCELERATION;
                    RectTransform.anchoredPosition = RectTransform.anchoredPosition + Vector2.down * fallSpeed * Time.deltaTime;

                    if (RectTransform.anchoredPosition.y - Node.RectTransform.anchoredPosition.y <= 0)
                    {
                        RectTransform.anchoredPosition = Node.RectTransform.anchoredPosition;
                        fallSpeed = 0;
                        animCollision.Play();
                        State = MainItemStates.GROUND;

                        if (Node.y == 0)
                            DOVirtual.DelayedCall(0.5f, () => { Explode(ExplodeConditions.BOTTOM_ROW); });
                    }
                }
            }
            else if(State == MainItemStates.FILL)
            {
                if (RectTransform.anchoredPosition.y - Node.RectTransform.anchoredPosition.y > 0)
                {
                    RectTransform.anchoredPosition = RectTransform.anchoredPosition + Vector2.down * Constants.FILL_SPEED * Time.deltaTime;

                    if (RectTransform.anchoredPosition.y - Node.RectTransform.anchoredPosition.y <= 0)
                    {
                        RectTransform.anchoredPosition = Node.RectTransform.anchoredPosition;
                        animCollision.Play();
                        State = MainItemStates.GROUND;

                        if (Node.y == 0)
                            DOVirtual.DelayedCall(0.5f, () => { Explode(ExplodeConditions.BOTTOM_ROW); });
                    }
                }
            }
        }
    }
}
