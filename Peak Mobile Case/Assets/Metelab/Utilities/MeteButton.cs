using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine;

namespace Metelab
{
    public class MeteButton: Button
    {
        public static event Action ActionOnGlobalClick;

        private RectTransform _RectTransform;
        public RectTransform RectTransfrom
        {
            get
            {
                if (_RectTransform == null)
                    _RectTransform = (RectTransform)transform;

                return _RectTransform;
            }
        }

        public void Init(Action OnClick)
        {
            onClick.RemoveAllListeners();
            onClick.AddListener(new UnityAction(OnClick));
            onClick.AddListener(GlobalClick);
        }


        private static void GlobalClick()
        {
            ActionOnGlobalClick?.Invoke();
        }

        public virtual void Active()
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactive()
        {
            gameObject.SetActive(false);
        }
    }
}