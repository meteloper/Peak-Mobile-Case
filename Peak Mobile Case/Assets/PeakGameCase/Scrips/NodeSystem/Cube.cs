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
        public VFXCubeBlast Prefab_VFX_CubeBlast;

        public static MeteObjectPool<VFXCubeBlast> objectPool_VFX_Blust = new MeteObjectPool<VFXCubeBlast>();   

        public override void Trigger()
        {
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;
        }

        public override void Explode()
        {
            gameObject.SetActive(false);
            objectPool_VFX_Blust.Instantiate(Prefab_VFX_CubeBlast, transform.position, Quaternion.identity).Play();
        }

    }
}
