using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Metelab
{
    public static class MeteExtensionMethods
    {
        /// <summary>
        /// All child will be destory but exception indexs will not be destory.
        /// </summary>
        public static void DestoryChildren(this Transform parent, params int[] exceptionIndexs)
        {
            if (parent == null || parent.childCount == 0)
                return;

            Transform transformChild;

            if (exceptionIndexs == null)
            {
                for (int i = parent.childCount - 1; i >= 0; i--)
                {
                    transformChild = parent.GetChild(i);
                    if (transformChild != null && transformChild.gameObject != null)
                        Object.Destroy(transformChild.gameObject);
                }
            }
            else
            {
                for (int i = parent.childCount - 1; i >= 0; i--)
                {
                    if (exceptionIndexs.Contains(i))
                        continue;

                    transformChild = parent.GetChild(i);
                    if (transformChild != null && transformChild.gameObject != null)
                        Object.Destroy(transformChild.gameObject);
                }
            }
        }

        public static List<T> InstantiateChildren<T>(this Transform parent, T prefab, int count, System.Action<T, int> forEachChild = null) where T : MonoBehaviour
        {
            if (count < 1)
                return null;

            List<T> addedChildList = new List<T>();

            for (int i = 0; i < count; i++)
            {
                addedChildList.Add(Object.Instantiate(prefab, parent));

                forEachChild?.Invoke(addedChildList[i], i);
            }

            return addedChildList;
        }

        public static Dictionary<string, string> GetLinkQueryDictionary(this string link)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string query = string.Empty;

            if (link.Contains('?'))
            {
                int questionMarkIndex = link.IndexOf('?');
                query = link.Substring(questionMarkIndex + 1, link.Length - questionMarkIndex - 1);
            }
            else
                query = link;

            if (!string.IsNullOrEmpty(query))
            {
                if (query.Contains('&')) // for multi parameters
                {
                    string[] allElements = query.Split('&');

                    string key, value;
                    for (int i = 0; i < allElements.Length; i++)
                    {
                        if (allElements[i].Contains('='))
                        {
                            int equalIndex = allElements[i].IndexOf('=');

                            key = allElements[i].Substring(0, equalIndex);
                            value = allElements[i].Substring(equalIndex + 1, allElements[i].Length - equalIndex - 1);

                            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                                parameters[key] = value;
                        }
                    }
                }
                else // for single parameter
                {
                    if (query.Contains('='))
                    {
                        int equalIndex = query.IndexOf('=');

                        string key = query.Substring(0, equalIndex);
                        string value = query.Substring(equalIndex + 1, query.Length - equalIndex - 1);

                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                            parameters[key] = value;
                    }
                }
            }

            return parameters;


            // LogsManager.Log("GetQueryDictionary :" + new List<string>(part).ToStringList((x,i) => { return x; }));
        }

        public static string GetLinkQuery(this Dictionary<string, string> queryDic)
        {
            string query = "";

            int index = 0;

            foreach (var item in queryDic)
            {
                query += $"{queryDic.Keys}={queryDic.Values}";

                if (index < queryDic.Count)
                    query += "&";
                else
                    index++;
            }

            return query;
        }

        public static string TruncateString(this string text, int maxLenght)
        {
            if (maxLenght > 3 && text.Length > maxLenght)
                return text.Substring(maxLenght - 3) + "...";

            return text;
        }

        public static Color SetAlpha(this Color color, float aplha)
        {
            color.a = aplha;
            return color;
        }


        public static Color SetColorWithoutAlpha(this Color color, Color newColor)
        {
            color.r = newColor.r;
            color.g = newColor.g;
            color.b = newColor.b;
            return color;
        }

        public static void SetAlpha(this Image image, float aplha)
        {
            image.color = image.color.SetAlpha(aplha);
        }

        public static void SetColorWithoutAlpha(this Image image, Color color)
        {
            color.a = image.color.a;
            image.color = color;
        }

        public static List<string> RandomMix(this List<string> list)
        {
            List<string> mixedList = new List<string>(list.Count);
            List<string> tempList = new List<string>(list);

            int randomIndex;
            for (int i = 0; i < list.Count; i++)
            {
                randomIndex = Random.Range(0, tempList.Count);
                mixedList.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);
            }

            return mixedList;
        }

        public static List<string> ToUpper(this List<string> list, CultureInfo cultureInfo = null)
        {
            List<string> result = new List<string>(list.Count);
            if (cultureInfo == null)
            {
                foreach (var item in list)
                    result.Add(item.ToUpper());
            }
            else
            {
                foreach (var item in list)
                    result.Add(item.ToUpper(cultureInfo));
            }

            return result;
        }

        public static List<string> ToLower(this List<string> list, CultureInfo cultureInfo = null)
        {
            List<string> result = new List<string>(list.Count);
            if (cultureInfo == null)
            {
                foreach (var item in list)
                    result.Add( item.ToLower());
            }
            else
            {
                foreach (var item in list)
                   result.Add( item.ToLower(cultureInfo));
            }

            return result;
        }

        public static string ToUpperFirst(this string text, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (cultureInfo == null)
                return text.Substring(0, 1).ToUpper() + text.Substring(1);
            else
                return text.Substring(0, 1).ToUpper(cultureInfo) + text.Substring(1);
        }

        public static T RandomSelect<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static T RandomSelect<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }


        public static T RandomSelectAndRemove<T>(this List<T> list)
        {
            int randomIndex = Random.Range(0, list.Count);
            T randomItem = list[randomIndex];
            list.RemoveAt(randomIndex);
            return randomItem;
        }

        public static T GetLastAndRemove<T>(this List<T> list)
        {
            int lastIndex = list.Count-1;
            T randomItem = list[lastIndex];
            list.RemoveAt(lastIndex);
            return randomItem;
        }

        public static T GetFirstAndRemove<T>(this List<T> list)
        {
            int firstIndex = 0;
            T randomItem = list[firstIndex];
            list.RemoveAt(firstIndex);
            return randomItem;
        }

        public static T GetLast<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static T GetFirst<T>(this List<T> list)
        {
            return list[0];
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        #region Array

        public static string ToStringFull<T>(this T[] array)
        {
            int len = array.Length;
            string str = "=>\n";
            T item;
            for (int i = 0; i < len; i++)
            {
                item = array[i];
                str += $"{i}:{item.ToString()}\n";
            }
            return str;
        }

        #endregion


        #region List

        public static string ToStringFull<T>(this List<T> list)
        {
            int len = list.Count;
            string str = "=>\n";
            T item;
            for (int i = 0; i < len; i++)
            {
                item = list[i];
                str += $"{i}:{item.ToString()}\n";
            }
            return str;
        }

        public static void AddIfNotContain<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        //public static void RemoveIfContain<T>(this List<T> list, T item)
        //{
        //    if (list.Contains(item))
        //        list.Remove(item);
        //}

        #endregion

        #region Dictionary

        public static void CreateOrAddValue<T>(this Dictionary<T,int> dic, T key ,int value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = dic[key] + value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        #endregion


        #region CanvasGroup

        public static void Open(this CanvasGroup canvas)
        {
            canvas.alpha = 1;
            canvas.blocksRaycasts = true;
            canvas.interactable = true;

            if (!canvas.gameObject.activeSelf)
                canvas.gameObject.SetActive(true);
        }

        public static void Close(this CanvasGroup canvas)
        {
            canvas.alpha = 0;
            canvas.blocksRaycasts = false;
            canvas.interactable = false;
        }

        #endregion
    }
}