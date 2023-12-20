using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Rocket : NodeItemDynamic
    {
        public VFXControllerBase PrefabMergeIntroEffect;
        public RectTransform RocketBody;
        private CanvasGroup CanvasGroup;



        public override void EarlyInit()
        {
            base.Init();
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        public void PlayMergeIntroEffects()
        {
            OP_VFXController.Instantiate(PrefabMergeIntroEffect, RectTransform.GetChild(0).transform.position, Quaternion.identity, transform).Play();
            RocketBody.localScale = Vector3.zero;
            CanvasGroup.alpha = 0;
            RocketBody.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            CanvasGroup.DOFade(1,0.4f).SetEase(Ease.OutBack);
        }
    }
}
