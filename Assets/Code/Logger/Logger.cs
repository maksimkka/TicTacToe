using UnityEngine;

namespace Code.Logger
{
    public static class Logger
    {
        public static void Mark()
        {
#if UNITY_EDITOR
            const string message = "(_MARK_)";
            InternalLog(message, LogType.Message, null);
#endif
        }
        
        public static void Separator()
        {
#if UNITY_EDITOR
            const string message = "============================================";
            InternalLog(message, LogType.Message, null);
#endif
        }

        public static void Log(this object message, Object context = null) => InternalLog(message, LogType.Message, context);

        public static void LogWarning(this object message, Object context = null) => InternalLog(message, LogType.Warning, context);
        
        public static void LogError(this object message, Object context = null) => InternalLog(message, LogType.Error, context);

        public static string Colored(this object message, Color color)
        {
#if UNITY_EDITOR
            var colorString = ColorUtility.ToHtmlStringRGB(color);
            var modifiedString = $"<color=#{colorString}>{message}</color>";
            return modifiedString;
#endif
            return string.Empty;
        }

        private static void InternalLog(this object message, LogType logType, Object context)
        {
#if UNITY_EDITOR
            switch (logType)
            {
                case LogType.Message:
                    Debug.Log(message, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case LogType.Error:
                    Debug.LogError(message, context);
                    break;
            }
#endif
        }
    }
}