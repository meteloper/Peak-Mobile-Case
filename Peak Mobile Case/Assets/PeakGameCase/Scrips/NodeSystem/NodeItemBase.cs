using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Metelab.PeakGameCase
{
    public abstract class NodeItemBase : MeteMono
    {
        public static MeteObjectPool<VFXControllerBase> OPExplodeEffect = new MeteObjectPool<VFXControllerBase>();

        public GridNode Node { get; set; }

        public ExplodeConditions ExplodeCondition;
        public TriggerConditions TriggerCondition;
        public NodeItemIds ItemId;
        public NodeItemTypes ItemType;
        public AudioNames ExplodeAudio;
        public VFXControllerBase PrefabExplodeEffect;

        public Action<NodeItemBase> OnTriggered;
        public Action<NodeItemBase, ExplodeConditions> OnExploded;
        public virtual void Explode(ExplodeConditions condition)
        {
            if (ExplodeCondition.HasFlag(condition))
            {
                gameObject.SetActive(false);
                Node.TakeDynamicItem();
                OPExplodeEffect.Instantiate(PrefabExplodeEffect, transform.position, Quaternion.identity, transform.parent.parent).Play();
                OnExploded?.Invoke(this, condition);
                AudioManager.Instance.PlayOneShot(ExplodeAudio);

                if (TriggerCondition.HasFlag(TriggerConditions.EXPLODE))
                {
                    Trigger();
                }
            }
        }

        public virtual void Trigger()
        {
           

            OnTriggered(this);
        }

    }
}
