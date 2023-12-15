using Metelab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public enum PanelNames:short
    {
        GamePanel = 0
    }


    public class PanelControllerMainScene : MetePanelControllerBase<PanelControllerMainScene, PanelNames>
    {
        public override short ConvertToIndex(PanelNames panelName)
        {
            return (short)panelName;
        }
    }
}

