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

        public abstract MetePanel ShowPanel(KEnum panelName);
        public abstract T ShowPanel<T>(KEnum panelName) where T : MetePanel;
        public abstract void HidePanel(KEnum panelName);
        public abstract MetePanel GetPanel(KEnum panelName);
        public abstract T GetPanel<T>(KEnum panelName) where T : MetePanel;

        protected MetePanel ShowPanel(short panel)
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

        protected T ShowPanel<T>(short panel) where T : MetePanel
        {
            return (T)ShowPanel(panel);
        }

        protected void HidePanel(short panel)
        {
            PanelList[(int)panel].HidePanel();

            if (ActivePanelList.Contains(panel))
                ActivePanelList.Remove(panel);

            if (CurrentActivePanel == panel)
                CurrentActivePanel = -1;
        }

        protected MetePanel GetPanel(short panel)
        {
            if (panel != -1)
                return PanelList[(int)panel];
            else
                return null;
        }

        protected T GetPanel<T>(short panel) where T : MetePanel
        {
            return (T)GetPanel(panel);
        }
    }
}
