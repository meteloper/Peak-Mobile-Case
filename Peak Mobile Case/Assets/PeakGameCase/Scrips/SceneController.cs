using Metelab.CommonManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metelab.PeakGameCase
{
    public class SceneController : MonoBehaviour
    {
        public string NextSceneName;
        public string StartSceneName;
        public MeteMono[] meteMono;

        void Awake()
        {
            if (GameManager.Instance == null)
            {
                SceneManager.LoadScene(StartSceneName);
            }
            else
            {
                foreach (var item in meteMono)
                {
                    item.EarlyInit();
                }
            }
        }

        private IEnumerator Start()
        {
            foreach (var item in meteMono)
            {
                item.Init();
            }

            yield return null;
            if (!string.IsNullOrEmpty(NextSceneName))
                SceneManager.LoadScene(NextSceneName);
        }
    }
}
