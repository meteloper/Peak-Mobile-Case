using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public enum MainItemStates
    {
        GROUND,
        FALL,
        FILL,
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

    public enum NodeItemTypes
    {
        CUBE,
        ROCKET,
        BALLOON,
        DUCK
    }

    [Flags]
    public enum ExplodeConditions
    {
        NONE = 0,
        CLICK = 1,
        MATCH = 2,
        MATCH_SIDE = 4,
        ROCKET = 8,
        BOTTOM_ROW = 16
       
    }

    [Flags]
    public enum TriggerConditions
    {
        NONE = 0,
        CLICK = 1, 
        EXPLODE = 2
    }

    public enum NodeItemIds:int
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
        ROCKET_RANDOM = -4,
        CUBE_RANDOM = -3,
        RANDOM = -2,
        SPACE = -1,

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

    public class Constants : MonoBehaviour
    {
        public const int CUBE_COUNT = 5;
        public const float FALL_ACCELERATION = 3000;
        public const float FILL_SPEED = 1250;
        public const float ROCKET_SPEED = 1250;
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
