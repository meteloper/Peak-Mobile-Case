using Metelab.PeakGameCase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metelab.PeakGameCase
{
    public class Cube : NodeItemBase
    {
        public Image image;

        public override void SetDebugColor()
        {
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;
        }
    }
}
