using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public enum GridItemType:int
    {
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4
    }

    public enum GridItemCreateType:int
    {
        CUBE_YELLOW = 0,
        CUBE_RED = 1,
        CUBE_BLUE = 2,
        CUBE_GREEN = 3,
        CUBE_PURPLE = 4,
        CUBE_RANDOM = 5,
        MAX = 6
    }

    public class Constants : MonoBehaviour
    {
        /// <summary>
        /// Left,Right,Bot,Top
        /// </summary>
        public static readonly Margin GRID_MARGINS = new Margin(16,16,16,36);
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