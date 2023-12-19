using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    public class VFXControllerParticle : VFXControllerBase
    {
        public new ParticleSystem particleSystem;

        public override void Play()
        {
            particleSystem.Play();
            base.Play();
        }

    }
}
