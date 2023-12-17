using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class VFXCubeBlast : MonoBehaviour
    {
        public new ParticleSystem particleSystem;
        public float LifeTime;


        public void Play()
        {
            particleSystem.Play();
            StartCoroutine(ILifeTime());
        }

        private IEnumerator ILifeTime()
        {
            yield return new WaitForSeconds(LifeTime);
            gameObject.SetActive(false);
        }
     


    }
}
