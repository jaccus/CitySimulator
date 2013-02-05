using System.Diagnostics;
using System.Globalization;

namespace City
{
    public static class Logger
    {
        public static void LogInfo(string msg)
        {
            Debug.WriteLine(msg);
        }

        public static void LogInfo(string msg, params object [] parameters)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, msg, parameters));
        }

        public static void LogWarning(string msg, params object[] parameters)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "WARNING: " + msg, parameters));
        }

        public static void LogError(string msg, params object[] parameters)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "ERROR: " + msg, parameters));
        }
    }
}