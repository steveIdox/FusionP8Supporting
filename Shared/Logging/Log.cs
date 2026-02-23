using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class Log
    {
        public static void Info(string message, string activityName) { Console.WriteLine($"{activityName}: {message}"); }
        public static void Error(string message, string activityName) { Console.WriteLine($"{activityName}: {message}"); }
        public static void Debug(string message, string activityName) { Console.WriteLine($"{activityName}: {message}"); }
    }
}
