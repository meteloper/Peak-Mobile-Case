using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab
{
    public abstract class MeteScriptableObject : ScriptableObject
    {
        public MeteLogSO LogData;

        public virtual void EarlyInit()
        {

        }

        public virtual void Init()
        {

        }

        public virtual void ResetData()
        {

        }
    }
}

