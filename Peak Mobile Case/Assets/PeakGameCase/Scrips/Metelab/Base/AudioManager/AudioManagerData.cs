using UnityEngine;

namespace Metelab.AudioManager_
{
    [CreateAssetMenu(fileName = "AudioManagerData", menuName = "Metelab/AudioManagerData")]
    public class AudioManagerData : MeteSingletonScriptableObject<AudioManagerData>
    {
        [SerializeField] private AudioClipData[] AudioClips;

        public AudioClipData GetAudioClipData(int index)
        {
            if (AudioClips != null && AudioClips.Length > index)
                return AudioClips[Mathf.Clamp(index, 0, AudioClips.Length)];
            else
                return null; 
        }
    }

    [System.Serializable]
    public class AudioClipData
    {
        public AudioClip Clip;
        [Range(0f,1f)]
        public float RelativeVolume = 1f;
    }
}

