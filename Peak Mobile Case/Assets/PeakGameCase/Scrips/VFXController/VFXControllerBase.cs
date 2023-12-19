using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public abstract class VFXControllerBase : MonoBehaviour
    {
        public float LifeTime;
        public virtual void Play()
        {
            StartCoroutine(ILifeTime());
        }

        private IEnumerator ILifeTime()
        {
            yield return new WaitForSeconds(LifeTime);
            gameObject.SetActive(false);
        }
    }
}
