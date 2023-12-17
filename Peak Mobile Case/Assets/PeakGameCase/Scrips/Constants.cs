using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public enum GameStates
    {
        CAN_MOVE,
        MOVE_STARTED
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

    public enum NodeItemType
    {
        CUBE,
        ROCKET,
        BALLOON,
        DUCK
    }

    public enum NodeItemId:int
    {
        NONE = -1,
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4,
        ROCKET_HORIZONTAL = 5,
        ROCKET_VERTICAL = 6,
        BALLOON = 7,
        DUCK = 8,

        MAX = 9
    }

    public enum NodeItemCreateId:int
    {
        EMPTY = -1,
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4,
        CUBE_RANDOM = 5,
        ROCKET_HORIZONTAL = 6,
        ROCKET_VERTICAL = 7,
        ROCKET_RANDOM = 8,
        BALLOON = 9,
        DUCK = 10,
        RANDOM = 11
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
