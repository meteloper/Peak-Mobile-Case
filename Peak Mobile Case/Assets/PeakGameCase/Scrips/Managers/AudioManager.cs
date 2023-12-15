using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Metelab.PeakGameCase
{
    public enum AudioNames:int
    {
        Audio1,
        Audio2
    }


    public class AudioManager : MeteAudioManagerBase<AudioManager, AudioNames>
    {
        public override int ConvertToInt(AudioNames audio)
        {
            return (int)audio;
        }
    }
}
