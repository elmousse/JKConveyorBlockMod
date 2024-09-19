#if DEBUG
using System;
using System.Globalization;
using System.IO;

namespace ConveyorBlockMod.Utils
{
    public static class Logger
    {
        private const string LogFilePath = "mod_log.txt";
        private static DateTime _startTime;

        public static void StartTimer()
        {
            _startTime = DateTime.Now;
        }

        public static void Log(string message)
        {
            var elapsed = DateTime.Now - _startTime;
            var elapsedTime = $"{elapsed.TotalSeconds.ToString("000.000", CultureInfo.InvariantCulture)}";
            using (var writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"{elapsedTime} | {message}");
            }
        }

        public static void EraseAll()
        {
            File.WriteAllText(LogFilePath, string.Empty);
        }

        public static void Return()
        {
            Log(string.Empty);
        }
    }
}
#endif