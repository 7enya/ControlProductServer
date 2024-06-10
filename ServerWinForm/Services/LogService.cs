using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWinForm.Services
{
    public static class LogService
    {
        private static Semaphore writeSem = new Semaphore(1, 1);
        private static Logger logger { get; set; }

        public static void Initialize()
        {
            var target = new FileTarget();
            target.FileName = "${basedir}/logs/${shortdate}.log";
            target.Name = "Logger";
            target.ArchiveFileName = "${basedir}/logs/archive/archive.${shortdate}.{#}.txt";
            target.ArchiveNumbering = ArchiveNumberingMode.Date;
            target.MaxArchiveFiles = 10;
            target.MaxArchiveDays = 30;
            target.Layout = "${date:format=yyyy-MM-dd HH\\:MM\\:ss}|${level:uppercase=true}| ${message}";
            var config = new LoggingConfiguration();
            config.AddTarget(target);
            config.AddRuleForAllLevels(target);
            LogManager.Configuration = config;
            LogManager.AutoShutdown = true;
            logger = LogManager.GetCurrentClassLogger();
        }

        public static void Write(LogLevel logLevel, string message) 
        {
            writeSem.WaitOne();
            logger.Log(logLevel, message);
            writeSem.Release();
        }
        
    }
}
