using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metelab.AudioManager_;
using System;

namespace Metelab
{
    public abstract class MeteAudioManagerBase<K,KEnum>  : MeteSingleton<MeteAudioManagerBase<K, KEnum>>
        where K : MeteAudioManagerBase<K, KEnum>
        where KEnum : Enum
    {
        public AudioManagerSO AudioManagerData;

        [SerializeField] private List<AudioSource> ListDeactiveAudioSource = new();
        [SerializeField] private List<AudioSource> ListActiveAudioSource = new();
        [SerializeField] private Dictionary<KEnum, AudioSource> DicLoopAudioSource = new();
     
        private int mAudioSourceCount;
        private Dictionary<KEnum, float> DicAudioToReadyTime = new Dictionary<KEnum, float>();

        /// <summary>
        /// Work like Awake
        /// </summary>
        public override void EarlyInit()
        {
            base.EarlyInit();
            mAudioSourceCount = 0;
            AudioManagerData.EarlyInit();
        }

        /// <summary>
        /// Work like Start
        /// </summary>
        public override void Init()
        {
            base.Init();
            AudioManagerData.Init();
        }

        public abstract int ConvertToInt(KEnum audio);


        public void SetMuteActives(bool isMute)
        {
            foreach (var audioSource in ListActiveAudioSource)
            {
                if (audioSource != null)
                    audioSource.mute = isMute;
            }
        }

        public void SetMuteLoops(bool isMute)
        {
            foreach (KeyValuePair<KEnum, AudioSource> item in DicLoopAudioSource)
            {
                if (item.Value != null)
                    item.Value.mute = isMute;
            }
        }

        public void PlayOneShot(KEnum audio)
        {
            StartCoroutine(IPlayOneShot(audio));
        }

        private bool IsCooldownFinished(KEnum audio)
        {
            if (DicAudioToReadyTime.ContainsKey(audio))
                return Time.time >= DicAudioToReadyTime[audio];
            else
                return true;
        }

        private void SetCooldown(KEnum audio, float cooldown)
        {
            if (cooldown > 0)
                DicAudioToReadyTime[audio] = Time.time + cooldown;
        }


        private IEnumerator IPlayOneShot(KEnum audio)
        {
            AudioSource audioSource;
            AudioClipData audioClipData = AudioManagerData.GetAudioClipData(ConvertToInt(audio));
            if (audioClipData == null)
                yield break;

            if (!IsCooldownFinished(audio))
                yield break;

                Metelab.Log(this, $"IPlayOneShot-first-audio:{audio}, ListAudioSource:{ListDeactiveAudioSource.Count}");
            if (ListDeactiveAudioSource.Count > 0)
            {
                audioSource = ListDeactiveAudioSource[0];
                //Metelab.Log(this, $"IPlayOneShot-audioSource-clipName:{audioSource.clip.name}, name{audioSource.name}");
                ListDeactiveAudioSource.RemoveAt(0);
            }
            else
            {
                audioSource = CreateNewAudioSource();
            }

            ListActiveAudioSource.Add(audioSource);
            //Metelab.Log(this, $"IPlayOneShot-AudioSource Name :{audioSource.name}");

            audioSource.volume = audioClipData.RelativeVolume;
            audioSource.loop = false;
            audioSource.clip = audioClipData.Clip;
           // audioSource.mute = !SettingsData.Instance.Sound;
            audioSource.Play();
            SetCooldown(audio, audioClipData.Cooldown);
            yield return new WaitForSecondsRealtime(audioSource.clip.length);

            if(ListActiveAudioSource.Contains(audioSource))
                ListActiveAudioSource.Remove(audioSource);

            if(!ListDeactiveAudioSource.Contains(audioSource))
                ListDeactiveAudioSource.Add(audioSource);

            //Metelab.Log(this, $"IPlayOneShot-last-clipName:{audioSource.clip.name}, Dict:{DicLoopAudioSource.Count}, List:{ListDeactiveAudioSource.Count}, Active:{ListActiveAudioSource.Count}");
        }

        public void PlayLoop(KEnum audio)
        {
            Metelab.Log(this, $"PlayLoop-first-Dict:{DicLoopAudioSource.Count}, List:{ListDeactiveAudioSource.Count}, Active:{ListActiveAudioSource.Count}");
            Metelab.Log(this, $"PlayLoop-audio:{audio}");
            AudioSource audioSource;
            AudioClipData audioClipData = AudioManagerData.GetAudioClipData(ConvertToInt(audio));
            if (audioClipData == null)
                return;

            if (DicLoopAudioSource.ContainsKey(audio))
            {
                Metelab.Log(this, "PlayLoop-audio is playing!!!");
                audioSource = DicLoopAudioSource[audio];
                if (audioSource.isPlaying)
                    return;
            }
            else
            {
                Metelab.Log(this, $"PlayLoop-audio:{audio}, ListAudioSource:{ListDeactiveAudioSource.Count}");
                if (ListDeactiveAudioSource.Count > 0)
                {
                    audioSource = ListDeactiveAudioSource[0];
                    Metelab.Log(this, $"PlayLoop-audioSource-clipName:{audioSource.clip.name}, name{audioSource.name}");
                    ListDeactiveAudioSource.RemoveAt(0);
                }
                else
                {
                    audioSource = CreateNewAudioSource();
                }
            }

            Metelab.Log(this, $"PlayLoop-AudioSource Name :{audioSource.name}");
            DicLoopAudioSource.Add(audio, audioSource);
            audioSource.volume = audioClipData.RelativeVolume;
            audioSource.loop = true;
            audioSource.clip = audioClipData.Clip;
           // audioSource.mute = !SettingsData.Instance.Music;
            audioSource.Play();
            Metelab.Log(this, $"PlayLoop-last-Dict:{DicLoopAudioSource.Count}, List:{ListDeactiveAudioSource.Count}, Active:{ListActiveAudioSource.Count}");
        }

        public void StopLoop(KEnum audio)
        {
            Metelab.Log(this, $"StopLoop-audio:{audio}");
            if (DicLoopAudioSource.ContainsKey(audio))
            {
                AudioSource audioSource = DicLoopAudioSource[audio];
                audioSource.Stop();
                DicLoopAudioSource.Remove(audio);
                if (!ListDeactiveAudioSource.Contains(audioSource))
                    ListDeactiveAudioSource.Add(audioSource);
            }
        }

        private void StopAllLoop()
        {
            foreach (KeyValuePair<KEnum, AudioSource> item in DicLoopAudioSource)
            {
                if (item.Value != null)
                {
                    item.Value.Stop();
                    if (!ListDeactiveAudioSource.Contains(item.Value))
                        ListDeactiveAudioSource.Add(item.Value);
                }
            }
            DicLoopAudioSource.Clear();
        }


        public void SlowStopLoop(KEnum audio, float stopTimeSec)
        {
            Metelab.Log(this, $"SlowStopLoop-audio:{audio}");
            StartCoroutine(ISlowStopLoop(audio, stopTimeSec));
        }

        private IEnumerator ISlowStopLoop(KEnum audio, float stopTimeSec)
        {
            if (DicLoopAudioSource.ContainsKey(audio))
            {
                AudioSource audioSource = DicLoopAudioSource[audio];
                float startVolume = audioSource.volume;
                float timer = 0;
                while (audioSource.volume > 0)
                {
                    audioSource.volume = startVolume*(1-(timer/ stopTimeSec));
                    timer += Time.deltaTime;
                    yield return null;
                }

                audioSource.Stop();
                DicLoopAudioSource.Remove(audio);

                if(!ListDeactiveAudioSource.Contains(audioSource))
                    ListDeactiveAudioSource.Add(audioSource);
            }
        }

        public void StopAll()
        {
            Metelab.Log(this, $"StopAll-before-Dict:{DicLoopAudioSource.Count}, List:{ListDeactiveAudioSource.Count}, Active:{ListActiveAudioSource.Count}");

            StopAllLoop();

            foreach (AudioSource item in ListActiveAudioSource)
            {
                if (item != null)
                {
                    item.Stop();
                    if (!ListDeactiveAudioSource.Contains(item))
                        ListDeactiveAudioSource.Add(item);
                }
            }
            ListActiveAudioSource.Clear();
            Metelab.Log(this, $"StopAll-after-Dict:{DicLoopAudioSource.Count}, List:{ListDeactiveAudioSource.Count}, Active:{ListActiveAudioSource.Count}");
        }

        private AudioSource CreateNewAudioSource()
        {
            AudioSource audioSource = new GameObject($"AS_{mAudioSourceCount}", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSource.transform.parent = transform;
            mAudioSourceCount++;
            return audioSource;
        }

        private void ChangeBackgroundMusic(KEnum audio)
        {
            Metelab.Log(this, $"audio:{audio}");
            StopAllLoop();
            PlayLoop(audio);
        }


        #region Events

        //private void OnGlobalClick()
        //{
        //    PlayOneShot(Audios.Hit2);
        //}

        //private void OnChangedGameMusic(Audios audio)
        //{
        //    ChangeBackgroundMusic(audio);
        //}

        #endregion
    }
}
