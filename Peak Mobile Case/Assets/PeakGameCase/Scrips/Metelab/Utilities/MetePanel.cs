using UnityEngine;
using Metelab.Panels;


namespace Metelab
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MetePanel : MeteMono
    {
 
        public Vector2 SafeAreaAnchorMin
        {
            get
            {
                return PanelSafeArea.anchorMin;
            }
        }

        public Vector2 SafeAreaAnchorMax
        {
            get
            {
                return PanelSafeArea.anchorMax;
            }
        }


        private CanvasGroup mCanvasGroup;
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (mCanvasGroup == null)
                    mCanvasGroup = GetComponent<CanvasGroup>();

                return mCanvasGroup;
            }
        }
        [Header("LudumPanel")]
        public MetePanelSO PanelData;
        public RectTransform PanelSafeArea;
        private Vector2 PanelBaseAnchorMin;
        private Vector2 PanelBaseAnchorMax;

        public bool IsOpen { get; private set; }

        public override void EarlyInit()
        {
            base.EarlyInit();
            PanelBaseAnchorMin = PanelSafeArea.anchorMin;
            PanelBaseAnchorMax = PanelSafeArea.anchorMax;
        }

        public override void Init()
        {
            base.Init();

            SetSafeArea();
        }

        public virtual void ShowPanel()
        {
            Active();
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            transform.SetAsLastSibling();
            RefreshPanel();
        }

        public virtual void HidePanel()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            IsOpen = false;
        }

        public virtual void RefreshPanel()
        {

        }

        public override void Active()
        {
            base.Active();
            IsOpen = true;
        }


        public override void Deactive()
        {
            base.Deactive();
            IsOpen = false;
        }

        private void SetSafeArea()
        {
            //for test
            //if (Screen.width.Equals(1125))
            //    ApplySafeArea(new Rect(0, 102, 1125, 2202)); // IPhone-X 1125x2436

            ApplySafeArea(Screen.safeArea);
        }

        private void ApplySafeArea(Rect r)
        {
            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;

            anchorMin.x /= Screen.width;
            if (PanelData.DownSafe)
                anchorMin.y /= Screen.height;
            else
                anchorMin.y = PanelBaseAnchorMin.y;


            anchorMax.x /= Screen.width;
            if (PanelData.UpSafe)
                anchorMax.y /= Screen.height;
            else
                anchorMax.y = PanelBaseAnchorMax.y;

            PanelSafeArea.anchorMin = anchorMin * 0.5f;
            PanelSafeArea.anchorMax = anchorMax;

            //LogsManager.LogFormat(name + "New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}", name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}