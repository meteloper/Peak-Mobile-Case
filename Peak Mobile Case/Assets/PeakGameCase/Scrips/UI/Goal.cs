using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Metelab.PeakGameCase
{
    public class Goal: MeteMono
    {
        public ParticleSystem CountParticleEffect;
        public Image ImageIcon;
        public TextMeshProUGUI TextCount;
        public RectTransform GoalItemParents;
       

        [SerializeField] private int countdown;

        private int realtimeCount;
        public int RealtimeCount
        {
            get { return realtimeCount; }
            set
            {
                if (value >= 0)
                    realtimeCount = value;
                else
                    realtimeCount = 0;
            }
        }

        public int Countdown
        {
            get
            {
                return countdown;
            }
            set
            {
                if(value >= 0)
                    countdown = value;
                else
                    countdown = 0;

                AudioManager.Instance.PlayOneShot(AudioNames.CubeCollect);
                CountParticleEffect.Play();
                TextCount.text = countdown.ToString();
            }
        }

        public GoalSO GoalSO;

        public void StartGoal(GoalSO goalSO)
        {
            this.GoalSO = goalSO;
            countdown = goalSO.Count;
            RealtimeCount = countdown;
            TextCount.text = countdown.ToString();
            ImageIcon.sprite = goalSO.GoalIcon;
        }
    }
}
