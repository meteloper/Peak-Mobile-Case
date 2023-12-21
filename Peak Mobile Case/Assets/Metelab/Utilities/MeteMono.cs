using UnityEngine;
namespace Metelab
{
    public abstract class MeteMono : MonoBehaviour
    {
        [Header(nameof(MeteMono))]
        public MeteLogSO LogData;

        public virtual void EarlyInit()
        {
           
        }

        public virtual void Init()
        {

        }

        public virtual void Active()
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactive()
        {
            gameObject.SetActive(false);
        }

        private RectTransform mRectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if(mRectTransform == null)
                    mRectTransform = GetComponent<RectTransform>();
                
                return mRectTransform;
            }
        }
    }
}