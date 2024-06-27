using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityUtil.Singleton;

namespace UnityUtil
{
    public class LogService : ClassSingleton<LogService>
    {
        public enum LogLevel
        {
            INFO = 0,
            WARNING = 1,
            ERROR = 2,
        }
        public enum LogType
        {
            UNITY_EDITOR,
            UNITY_WINDOWS,
            UNITY_ANDROID,
            UNITY_ALLPLATFORM
        }

        readonly string pathAll = Application.persistentDataPath +"/Log";
        FileStream pathInfoFile = null;
        FileStream pathWarningFile = null;
        FileStream pathErrorFile = null;
        FileStream pathLogFile = null;
        byte[] writetmp = null;
        LogLevel fileLogLevel = LogLevel.WARNING;
        LogLevel stackLogLevel = LogLevel.ERROR;
        LogType nowType = LogType.UNITY_ALLPLATFORM;

        public LogService()
        {
            string timeDir = pathAll+"/" + DateTime.Now.ToString("yyyyMMdd-HH_mm_ss");
            if (!Directory.Exists(timeDir))
            {
                Directory.CreateDirectory(timeDir);
            }
            pathInfoFile = File.Open(timeDir+"/Info.txt",FileMode.OpenOrCreate);
            pathWarningFile = File.Open(timeDir + "/Warning.txt", FileMode.OpenOrCreate);
            pathErrorFile = File.Open(timeDir + "/Error.txt", FileMode.OpenOrCreate);
            pathLogFile = File.Open(timeDir + "/All.txt", FileMode.OpenOrCreate);

        }

        private void WriteLog(LogLevel level, string ss, LogType type = LogType.UNITY_ALLPLATFORM)
        {
            if (type != LogType.UNITY_ALLPLATFORM && nowType != type) return;
            FileStream fs = null;
            switch (level) {
                case LogLevel.INFO: {fs = pathInfoFile;} break;
                case LogLevel.WARNING:{ fs = pathWarningFile; }break;
                case LogLevel.ERROR: { fs = pathErrorFile; } break;
            }
            fs.Seek(0, SeekOrigin.End);
            if (level >= stackLogLevel) {
                ss += new System.Diagnostics.StackTrace().ToString();
            }
            writetmp = System.Text.Encoding.Unicode.GetBytes(string.Format("{0}>>{1}:{2}\r\n", DateTime.Now.ToString().Replace('\\', '_'), level.ToString(), ss));
            fs.Write(writetmp,0, writetmp.Length);
            pathLogFile.Write(writetmp, 0, writetmp.Length);
            fs.Flush();
            pathInfoFile.Flush();
        }
        ~LogService()
        { 
            pathInfoFile.Dispose();
            pathWarningFile.Dispose();
            pathErrorFile.Dispose();
            pathLogFile.Dispose();
        }
        public static void setFileLogLevel(LogLevel level) {
            Instance.fileLogLevel = level;
        }
        public static void setFileLogType(LogType type)
        {
            Instance.nowType = type;
        }
        public static void LogInfo(string str, LogType type = LogType.UNITY_ALLPLATFORM)
        {
            UnityEngine.Debug.Log(str);
            Instance.WriteLog(LogLevel.INFO, str, type);
        }
        public static void LogWarning(string str, LogType type = LogType.UNITY_ALLPLATFORM)
        {
            UnityEngine.Debug.LogWarning(str);
            Instance.WriteLog(LogLevel.WARNING, str, type);
        }
        public static void LogError(string str, LogType type = LogType.UNITY_ALLPLATFORM)
        {
            UnityEngine.Debug.LogError(str);
            Instance.WriteLog(LogLevel.ERROR, str, type);
        }
    }
}
