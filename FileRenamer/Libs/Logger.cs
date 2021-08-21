using System;

namespace FileRenamer.Libs
{
    public static class Logger
    {
        public static void Info(object data)
        {
            Console.WriteLine("[Info] " + (data ?? "null"));
        }

        public static void Debug(object data)
        {
            Console.WriteLine("[Debug] " + (data ?? "null"));
        }

        public static void Error(object data)
        {
            Console.WriteLine("[Error] " + (data ?? "null"));
        }
    }
}