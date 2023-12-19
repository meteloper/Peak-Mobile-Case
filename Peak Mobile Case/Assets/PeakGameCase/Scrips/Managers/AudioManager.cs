using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    public enum AudioNames:int
    {
        None = -1,
        CubeExplode = 0,
        CubeCollect = 1,
        Balloon = 2,
        Duck = 3
    }


    public class AudioManager : MeteAudioManagerBase<AudioManager, AudioNames>
    {
        public override int ConvertToInt(AudioNames audio)
        {
            return (int)audio;
        }
    }
}
