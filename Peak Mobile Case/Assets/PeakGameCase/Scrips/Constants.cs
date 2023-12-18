using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public enum MainItemStates
    {
        FALL,
        GROUND,
        FILL,
    }

    public enum ConstraintType
    {
        TIME,
        ACCELERATION
    }

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
        CUBE_MAX = 5,

        ROCKET_HORIZONTAL = 6,
        ROCKET_VERTICAL = 7,
        BALLOON = 8,
        DUCK = 9
    }

    public enum NodeItemCreateId:int
    {
        SPACE = -1,
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
        public const float FALL_ACCELERATION = 3000;
        public const float FILL_SPEED = 1250;
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
