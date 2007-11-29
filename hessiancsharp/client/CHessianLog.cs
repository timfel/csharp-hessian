using System;
using System.Collections.Generic;
using System.Text;

namespace hessiancsharp.client
{

    public class CHessianLog
    {

        private const int MAX_ENTRIES = 200;
        private static bool LOGGING_ENABLED = false;

        public static bool LoggingEnabled
        {
            get { return LOGGING_ENABLED; }
            set { LOGGING_ENABLED = value; }
        }

        private static List<CHessianLogEntry> LOG_ENTRIES = new List<CHessianLogEntry>();

        public static void AddLogEntry(string methodName, DateTime start, DateTime finish, long bytesIn, long bytesOut)
        {
            if (LoggingEnabled)
            {
                if (LOG_ENTRIES.Count >= MAX_ENTRIES)
                    LOG_ENTRIES.RemoveAt(0);
                LOG_ENTRIES.Add(new CHessianLogEntry(methodName, start, finish, bytesIn, bytesOut));
            }
        }

        public static List<CHessianLogEntry> GetLog()
        {
            return LOG_ENTRIES;
        }

    }

    public class CHessianLogEntry
    {

        public CHessianLogEntry(string name, DateTime start, DateTime stop, long bytesIn, long bytesOut)
        {
            MethodName = name;
            ExecutionStart = start;
            ExecutionFinish = stop;
            BytesIn = bytesIn;
            BytesOut = bytesOut;
        }

        private string methodName;

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        private DateTime executionStart;

        public DateTime ExecutionStart
        {
            get { return executionStart; }
            set { executionStart = value; }
        }

        private DateTime executionFinish;

        public DateTime ExecutionFinish
        {
            get { return executionFinish; }
            set { executionFinish = value; }
        }

        public int ExecutionDuration
        {
            get {
                TimeSpan duration = ExecutionFinish - ExecutionStart;
                return (int)duration.TotalMilliseconds;
            }
        }

        private long bytesIn;

        public long BytesIn
        {
            get { return bytesIn; }
            set { bytesIn = value; }
        }

        private long bytesOut;

        public long BytesOut
        {
            get { return bytesOut; }
            set { bytesOut = value; }
        }
	
    }

}
