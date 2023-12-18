using DG.Tweening;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    [RequireComponent(typeof(Animation))]
    public class NodeDynamicItem : NodeItemBase
    {
        public static MeteObjectPool<ParticleController> OPExplodeEffect = new MeteObjectPool<ParticleController>();
  
        public MainItemStates State;
        public ParticleController PrefabExplodeEffect;

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
                        {
                            DOVirtual.DelayedCall(0.5f, () => { Explode(ExplodeConditions.BOTTOM_ROW); });
                        }
                           
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
                        {
                            DOVirtual.DelayedCall(0.5f,() => { Explode(ExplodeConditions.BOTTOM_ROW); });
                        }
                            
                    }
                }
            }
        }

        public override void Explode(ExplodeConditions condition)
        {
            if(ExplodeCondition == condition)
            {
                gameObject.SetActive(false);
                Node.TakeDynamicItem();
                OPExplodeEffect.Instantiate(PrefabExplodeEffect, transform.position, Quaternion.identity).Play();
                OnTriggered?.Invoke(this);
            }
        }

    }
}
