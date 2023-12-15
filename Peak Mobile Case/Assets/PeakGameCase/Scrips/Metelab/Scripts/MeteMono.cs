using UnityEngine;
namespace Metelab
{
    public abstract class MeteMono : MonoBehaviour
    {
        [Header(nameof(MeteMono))]
        public MeteLogData LogData;

        public virtual void EarlyInit()
        {
           
        }

        public virtual void Init()
        {

        }

        public virtual void Active()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public virtual void Deactive()
        {
            if (gameObject.activeSelf)
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