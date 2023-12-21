using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class LevelSelectionButton : MeteMono
    {
        public GridSO GridSO;


        public void StartLevel()
        {
            GamePanel gamePanel = PanelControllerMainScene.current.GetPanel<GamePanel>(PanelNames.GamePanel);
            PanelControllerMainScene.current.ShowPanel( PanelNames.GamePanel);
            gamePanel.StartGame(GridSO);

        }

    }

}
