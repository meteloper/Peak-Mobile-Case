using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metelab
{
    public class MeteObjectPool<T> where T : MonoBehaviour
    {
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
                Metelab.Log($"New text created!", Color.green);
            }
            else
            {
                instance.transform.position = startPos;
                instance.transform.rotation = quaternion;
                instance.transform.parent = parent;
                Metelab.Log($"Old text was used", Color.blue);
            }

            instance.gameObject.SetActive(true);

            return instance;
        }


    }

}
