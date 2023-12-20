using DG.Tweening;
using UnityEngine;

namespace Metelab.PeakGameCase
{

    public class NodeItemDynamic :NodeItem
    {
        public MainItemStates State;
   
        private float fallSpeed = 0;

        public Cube Cube
        {
            get
            {
                if (ItemType == NodeItemTypes.CUBE)
                    return (Cube)this;

                return null;
            }
        }

        public Rocket Rocket
        {
            get
            {
                if (ItemType == NodeItemTypes.ROCKET)
                    return (Rocket)this;

                return null;
            }
        }



        private void Update()
        {
            if(State == MainItemStates.FALL)
            {
                if (RectTransform.anchoredPosition.y - GridNode.RectTransform.anchoredPosition.y > 0)
                {
                    fallSpeed += Time.deltaTime * Constants.FALL_ACCELERATION;
                    RectTransform.anchoredPosition = RectTransform.anchoredPosition + Vector2.down * fallSpeed * Time.deltaTime;

                    if (RectTransform.anchoredPosition.y - GridNode.RectTransform.anchoredPosition.y <= 0)
                    {
                        RectTransform.anchoredPosition = GridNode.RectTransform.anchoredPosition;
                        fallSpeed = 0;
                        PlayCollision();
                        State = MainItemStates.GROUND;

                        if (GridNode.y == 0)
                            DOVirtual.DelayedCall(0.5f, () => { Explode(ExplodeConditions.BOTTOM_ROW); });
                    }
                }
            }
            else if(State == MainItemStates.FILL)
            {
                if (RectTransform.anchoredPosition.y - GridNode.RectTransform.anchoredPosition.y > 0)
                {
                    RectTransform.anchoredPosition = RectTransform.anchoredPosition + Vector2.down * Constants.FILL_SPEED * Time.deltaTime;

                    if (RectTransform.anchoredPosition.y - GridNode.RectTransform.anchoredPosition.y <= 0)
                    {
                        RectTransform.anchoredPosition = GridNode.RectTransform.anchoredPosition;
                        PlayCollision();
                        State = MainItemStates.GROUND;

                        if (GridNode.y == 0)
                            DOVirtual.DelayedCall(0.5f, () => { Explode(ExplodeConditions.BOTTOM_ROW); });
                    }
                }
            }
        }
    }
}
