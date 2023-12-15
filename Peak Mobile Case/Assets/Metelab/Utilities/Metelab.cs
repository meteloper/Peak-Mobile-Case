using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace Metelab
{
    public class Metelab
    {
        #region LOGS

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"<color=#FFFFFF>{message}</color>");
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(string message,Color color)
        {
            UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"<color=#FFFFFF>{message}</color>");
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void LogError(string message, Color color)
        {
            UnityEngine.Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"<color=#FFFFFF>{message}</color>");
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message, Color color)
        {
            UnityEngine.Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
        }

        #region MeteMono
        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteMono obj)
        {
            if(obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                {
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(obj.LogData.Color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()}</color>", obj);
                } 
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()}", obj);
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteMono obj, string message)
        {
            if (obj.LogData != null)
            {


                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(obj.LogData.Color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()}: {message}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()} :{message}", obj);
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteMono obj,string message, Color color)
        {
            if (obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()}: {message}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()} :{message}", obj);
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteMono obj, Color color)
        {
            if (obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()}", obj);
        }
        #endregion

        #region MeteScripableObject

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteScriptableObject obj)
        {
            if (obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(obj.LogData.Color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()}", obj);
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteScriptableObject obj, string message)
        {
            if (obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(obj.LogData.Color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()} :{message}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()} :{message}", obj);
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteScriptableObject obj, string message, Color color)
        {
            if (obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()} :{message}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()} :{message}", obj);
        }

        [Conditional("ENABLE_LOGS"), Conditional("UNITY_EDITOR")]
        public static void Log(MeteScriptableObject obj, Color color)
        {
            if (obj.LogData != null)
            {
                if (!obj.LogData.IsDisabled)
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{GetFilterText(obj.LogData.Filter)}{obj.GetType().Name}-{GetCurrentMethod()}</color>", obj);
            }
            else
                UnityEngine.Debug.Log($"{obj.GetType().Name}-{GetCurrentMethod()}", obj);
        }

        #endregion

        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }

        private static string GetFilterText(string filter)
        {
            string filterText = string.Empty;
            if (!string.IsNullOrEmpty(filter))
                filterText = $"<size=16><b>{filter}: </b></size>";

            return filterText;
        }
    }
}

