using log4net;

namespace Common
{
    public static class Logger
    {
        private static readonly ILog Log =
                 LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        public static void Error(object message)
        {
            Log.Error(message);
        }

        public static void Debug(object message)
        {
            Log.Debug(message);
        }

        public static void Info(object message)
        {
            Log.Info(message);
        }
    }
}
