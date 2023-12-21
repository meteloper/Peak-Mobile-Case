using Metelab.PeakGameCase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovesBoard : MonoBehaviour
{


    public TextMeshProUGUI TextCountdown;


    private int countdown;


    public int Countdown
    {
        get { return countdown; }
        set
        {
            if (value > 0)
                countdown = value;
            else
            {
                countdown = 0;
                GameEvents.InvokeOnGameOver( EndGameResult.LOSE);
            }

            TextCountdown.text = countdown.ToString();
        }
    }

    public void SetMoves(int maxMoveCount)
    {
        Countdown = maxMoveCount;
    }

    private void Start()
    {
        GameEvents.OnStartedMove += OnStartedMove;
    }

    private void OnDestroy()
    {
        GameEvents.OnStartedMove -= OnStartedMove;
    }


    private void OnStartedMove()
    {
        Countdown--;
    }


}
