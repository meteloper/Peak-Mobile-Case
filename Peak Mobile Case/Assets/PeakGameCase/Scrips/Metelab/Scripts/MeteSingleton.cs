using UnityEngine;

namespace Metelab
{
    public abstract class MeteSingleton<T> : MeteMono where T : MeteSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        //GameObject obj = new GameObject();
                        //obj.name = typeof(T).Name;
                        //instance = obj.AddComponent<T>();

                        Metelab.LogWarning("Singleton object wasn't found!");

                        return null;
                    }
                }
                return instance;
            }
        }

        private static GameObject parent;

        public override void EarlyInit()
        {
            if (instance == null)
            {
                instance = this as T;

                if (parent == null)
                {
                    Transform parentT = this.gameObject.transform.parent;
                    if (parentT != null)
                    {
                        parent = parentT.gameObject;
                        DontDestroyOnLoad(parent);
                    }
                }

                Metelab.Log(this, "instance was initialized.");
            }
            else
            {
                if(this != instance)
                {
                    Metelab.Log(this, "was destroyed.");
                    Destroy(gameObject);
                }
            }
        }
    }

}
