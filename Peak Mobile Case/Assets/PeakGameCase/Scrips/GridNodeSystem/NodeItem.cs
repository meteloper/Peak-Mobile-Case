using UnityEngine;

namespace Metelab.PeakGameCase
{
    [RequireComponent(typeof(Animation))]
    public abstract class NodeItem : MeteMono
    {
        protected static MeteObjectPool<VFXControllerBase> OP_VFXController = new MeteObjectPool<VFXControllerBase>();

        public RectTransform Center;
        public ExplodeConditions ExplodeCondition;
        public TriggerConditions TriggerCondition;
        public GoalItemIds GoalItemId;
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

        public virtual bool IsCanExplode(ExplodeConditions condition)
        {
            return ExplodeCondition.HasFlag(condition);
        }

        public virtual void PlayExplode()
        {
            OP_VFXController.Instantiate(PrefabExplodeEffect, RectTransform.GetChild(0).transform.position, Quaternion.identity, transform.parent.parent).Play();
            AudioManager.Instance.PlayOneShot(ExplodeAudio);
        }

        public void PlayShake()
        {
            anim.Play("Shake");
        }

        public void PlayCollision()
        {
            anim.Play("Collision");
        }
    }
}
