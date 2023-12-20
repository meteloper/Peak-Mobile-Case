using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

namespace Metelab.PeakGameCase
{
    [RequireComponent(typeof(Animation))]
    public abstract class NodeItem : MeteMono
    {
        protected static MeteObjectPool<VFXControllerBase> OP_VFXController = new MeteObjectPool<VFXControllerBase>();

        public Action<NodeItem> OnTriggered;
        public Action<NodeItem, ExplodeConditions> OnExploded;

        public ExplodeConditions ExplodeCondition;
        public TriggerConditions TriggerCondition;
        public NodeItemIds ItemId;
        public NodeItemTypes ItemType;
        public AudioNames ExplodeAudio;
        public VFXControllerBase PrefabExplodeEffect;
        protected Animation anim;

        public GridNode GridNode { get; set; }

        public NodeItemDynamic NodeItemDynamic
        {
            get
            {
                return (NodeItemDynamic)this;
            }
        }

        public void Awake()
        {
            EarlyInit();
        }

        private void Start()
        {
            anim = GetComponent<Animation>();
            Init();
        }

        public virtual void Explode(ExplodeConditions condition)
        {
            if (ExplodeCondition.HasFlag(condition))
            {
                GameEvents.InvokeNodeItemExplode(this);

                if (!condition.HasFlag(ExplodeConditions.MERGE))
                {
                    gameObject.SetActive(false);
                    GridNode.TakeDynamicItem();
                    OP_VFXController.Instantiate(PrefabExplodeEffect, RectTransform.GetChild(0).transform.position, Quaternion.identity, transform.parent.parent).Play();
                    AudioManager.Instance.PlayOneShot(ExplodeAudio);
                }

               
                OnExploded?.Invoke(this, condition);
  
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

        public void PlayCollision()
        {
            anim.Play("Collision");
        }
    }
}
