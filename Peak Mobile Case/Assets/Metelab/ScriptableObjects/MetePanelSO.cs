using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metelab.Panels
{
    [CreateAssetMenu(fileName = "PanelData", menuName = "Metelab/PanelData")]
    public class MetePanelSO:MeteScriptableObject
    {
        public bool IsFullScreenPanel;
        public bool UpSafe;
        public bool DownSafe;

    }
}

