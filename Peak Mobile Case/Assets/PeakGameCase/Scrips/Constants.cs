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

    public enum Directions : int
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

    public enum ClickEffects
    {
        NONE,
        TRIGGER,
        EXPLODE
    }

    [Flags]
    public enum ExplodeConditions
    {
        NONE = 0,
        MATCH = 1,
        MATCH_SIDE = 2,
        ROCKET = 4,
        BOTTOM_ROW = 8,
        MERGE = 16,
        MERGE_SIDE = 32,
    }


    [Flags]
    public enum TriggerConditions
    {
        NONE = 0,
        ON_CLICK = 1,
        ON_EXPLODE = 2,
        ON_MERGE = 4
    }

    public enum NodeItemTypes:int
    {
        NONE = -1,
        CUBE = 0,
        ROCKET = 1,
        BALLOON = 2,
        TOY = 3
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
        BALLOON_BASIC = 7,
        TOY_DUCK = 8,
        MAX = 9
    }

    public enum NodeItemCreateIds:int
    {
        RANDOM_ROCKET = -4,
        RANDOM_CUBE = -3,
        RANDOM_ALL = -2,
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

    public enum GoalItemIds
    {
        NONE = -1,
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4,
        BALLOON_BASIC = 5,
        TOY_DUCK = 6
    }

    public enum EndGameResult
    {
        WIN,
        LOSE
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
