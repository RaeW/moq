using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Stunts.Tasks
{
    public class SetupEventLogger : Task
    {
        public bool Debug { get; set; }

        public override bool Execute()
        {
            if (Debug)
                Debugger.Launch();

            var lifetime = RegisteredTaskObjectLifetime.Build;
            var key = typeof(EventLogger).FullName;
            if (BuildEngine4.GetRegisteredTaskObject(key, lifetime) == null)
            {
                try
                {
                    var logger = new EventLogger();
                    var flags = BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public;
                    var context = BuildEngine.GetType().InvokeMember("LoggingContext", flags, null, BuildEngine, null);
                    var logging = context.GetType().InvokeMember("LoggingService", flags, null, context, null);
                    logging.GetType().InvokeMember("RegisterLogger", flags | BindingFlags.InvokeMethod, null, logging, new[] { logger });

                    BuildEngine4.RegisterTaskObject(key, logger, lifetime, false);
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex, true);
                    return false;
                }
            }

            return true;
        }
    }
}