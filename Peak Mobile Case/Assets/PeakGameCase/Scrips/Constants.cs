using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public enum GameStates
    {
        CanMove,
        MoveAnimatios
    }

    public enum Directions
    {
        UP = 0,
        UP_RIGHT = 1,
        RIGHT = 2,
        DOWN_RIGHT = 3,
        DOWN = 4,
        DOWN_LEFT = 5,
        LEFT = 6,
        UP_LEFT = 7,
        MAX = 8
    }

    public enum NodeItemTypes:int
    {
        NONE = -1,
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4,
        MAX = 5
    }

    public enum NodeItemCreateTypes:int
    {
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4,
        CUBE_RANDOM = 5,
        EMPTY = 6,
        MAX = 7
    }

    public class Constants : MonoBehaviour
    {

    }

    public struct Margin
    {
        public float left;
        public float right;
        public float bot;
        public float top;

        public Margin(float left,float right, float bot, float top)
        {
            this.left = left;   
            this.right = right; 
            this.bot = bot; 
            this.top = top; 
        }

    }

}
