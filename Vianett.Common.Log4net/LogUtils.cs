using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;
using System.Diagnostics;

namespace Vianett.Common.Log4net
{
    public static class LogUtils
    {
        public static void ConfigureTraceAndConsoleAppender()
        {
            log4net.Config.BasicConfigurator.Configure();
            log4net.Config.BasicConfigurator.Configure(new log4net.Appender.TraceAppender() { Layout = new log4net.Layout.PatternLayout("%date [%-5thread] [%property{NDC}] %-5level %logger - %message%newline") });
            //log4net.Config.BasicConfigurator.Configure(new log4net.Appender.ColoredConsoleAppender(new Vianett.Common.Log4net.Layout.DefaultFileLayout()));
        }

        public static Level GetLogLevel(string level)
        {
            level = (level ?? "").ToLower();
            switch (level)
            {
                case "finest": return Level.Finest;
                case "finer": return Level.Finer;
                case "fine": return Level.Fine;
                case "verbose": return Level.Verbose;
                case "trace": return Level.Trace;
                case "debug": return Level.Debug;
                case "info": return Level.Info;
                case "notice": return Level.Notice;
                case "warn": return Level.Warn;
                case "error": return Level.Error;
                case "severe": return Level.Severe;
                case "critical": return Level.Critical;
                case "alert": return Level.Alert;
                case "fatal": return Level.Fatal;
                case "emergency": return Level.Emergency;
                default:
                    Trace.WriteLine("Unknown log level: " + level);
                    return Level.All;
            }
        }
    }
}
