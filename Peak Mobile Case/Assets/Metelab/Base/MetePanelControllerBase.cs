using Metelab;
using Metelab.CommonManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Metelab
{
    public abstract class MetePanelControllerBase<K,KEnum> : MeteMono 
        where K : MetePanelControllerBase<K,KEnum> 
        where KEnum : Enum
    {
        public static K current;

        [Tooltip("It work if")]
        [SerializeField] private short StarterPanel = -1;
        [SerializeField] private List<MetePanel> PanelList = new List<MetePanel>();
        [SerializeField] private short CurrentActivePanel = -1;
        [SerializeField] private List<short> ActivePanelList = new List<short>();

        public override void EarlyInit()
        {
            base.EarlyInit();

            foreach (var panel in PanelList)
            {
                panel.EarlyInit();
                panel.Deactive();
            }

            current = (K)this;
        }


        public override void Init()
        {
            base.Init();

            foreach (var panel in PanelList)
                panel.Init();

            if (StarterPanel >= 0)
                ShowPanel(StarterPanel);
        }

        public abstract short ConvertToIndex(KEnum panelName);

        public virtual MetePanel ShowPanel(KEnum panelName)
        {
            return ShowPanel(ConvertToIndex(panelName));
        }
        public virtual T ShowPanel<T>(KEnum panelName) where T : MetePanel
        {
            return ShowPanel<T>(ConvertToIndex(panelName));
        }
        public virtual void HidePanel(KEnum panelName)
        {
            HidePanel(ConvertToIndex(panelName));
        }
        public virtual MetePanel GetPanel(KEnum panelName)
        {
            return GetPanel(ConvertToIndex(panelName));
        }
        public virtual T GetPanel<T>(KEnum panelName) where T : MetePanel
        {
            return GetPanel<T>(ConvertToIndex(panelName));
        }

        private MetePanel ShowPanel(short panel)
        {
            MetePanel targetPanel = PanelList[(int)panel];

            if (targetPanel.PanelData.IsFullScreenPanel)
            {
                if (CurrentActivePanel != -1 && CurrentActivePanel != panel)
                    HidePanel(CurrentActivePanel);

                CurrentActivePanel = panel;
            }

            if (!ActivePanelList.Contains(panel))
                ActivePanelList.Add(panel);

            targetPanel.ShowPanel();
            return targetPanel;
        }

        private T ShowPanel<T>(short panel) where T : MetePanel
        {
            return (T)ShowPanel(panel);
        }

        private void HidePanel(short panel)
        {
            PanelList[(int)panel].HidePanel();

            if (ActivePanelList.Contains(panel))
                ActivePanelList.Remove(panel);

            if (CurrentActivePanel == panel)
                CurrentActivePanel = -1;
        }

        private MetePanel GetPanel(short panel)
        {
            if (panel != -1)
                return PanelList[(int)panel];
            else
                return null;
        }

        private T GetPanel<T>(short panel) where T : MetePanel
        {
            return (T)GetPanel(panel);
        }
    }
}
