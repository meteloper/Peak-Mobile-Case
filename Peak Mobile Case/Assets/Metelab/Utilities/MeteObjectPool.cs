using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab
{
    public class MeteObjectPool<T> where T : MonoBehaviour
    {
        private List<GameObject> allPools = new List<GameObject>();
        private Dictionary<int, List<T>> dicPools = new();

        public T Instantiate(T prefab, Vector3 startPos, Quaternion quaternion,Transform parent = null)
        {
            int prefabInstanceID = prefab.GetInstanceID();
            if (!dicPools.ContainsKey(prefabInstanceID))
            {
                dicPools.Add(prefabInstanceID, new List<T>());
            }

            List<T> poolList = dicPools[prefabInstanceID];
            T instance = poolList.Find((item) => !item.gameObject.activeSelf);

            if (instance == null)
            {
                instance = MonoBehaviour.Instantiate(prefab, startPos, quaternion, parent);
                poolList.Add(instance);
                allPools.Add(instance.gameObject);
            }
            else
            {
                instance.transform.position = startPos;
                instance.transform.rotation = quaternion;
                instance.transform.parent = parent;
                instance.transform.SetAsLastSibling();
            }

            instance.gameObject.SetActive(true);

            

            return instance;
        }

        public T Instantiate(T prefab, Transform parent)
        {
            int prefabInstanceID = prefab.GetInstanceID();
            if (!dicPools.ContainsKey(prefabInstanceID))
            {
                dicPools.Add(prefabInstanceID, new List<T>());
            }

            List<T> poolList = dicPools[prefabInstanceID];
            T instance = poolList.Find((item) => !item.gameObject.activeSelf);

            if (instance == null)
            {
                instance = MonoBehaviour.Instantiate(prefab,parent);
                poolList.Add(instance);
                allPools.Add(instance.gameObject);
            }
            else
            {
                instance.transform.SetParent(parent);
                instance.transform.SetAsLastSibling();
            }
              

            instance.gameObject.SetActive(true);

            return instance;
        }

        public void DeactivePool()
        {
            foreach (var list in dicPools.Values)
            {
                foreach (var item in list)
                {
                    item.gameObject.SetActive(false);
                }
            }

        }


    }

}
