using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Stunts.Tasks
{
    public class CollectEvents : Task
    {
        IEnumerable<BuildEventArgs> events = Array.Empty<BuildEventArgs>();

        [Output]
        public ITaskItem[] Warnings => events.OfType<BuildWarningEventArgs>()
            .Select(w => new TaskItem(w.Code, new Dictionary<string, string>
            {
                { nameof(BuildWarningEventArgs.File), w.File },
                { nameof(BuildWarningEventArgs.LineNumber), w.LineNumber.ToString() },
                { nameof(BuildWarningEventArgs.ColumnNumber), w.ColumnNumber.ToString() },
                { nameof(BuildWarningEventArgs.Message), w.Message },
                { nameof(BuildWarningEventArgs.SenderName), w.SenderName },
                { nameof(BuildWarningEventArgs.ProjectFile), w.ProjectFile },
            })).ToArray();

        public bool Debug { get; set; }

        public bool Dispose { get; set; }

        public override bool Execute()
        {
            if (Debug)
                Debugger.Launch();

            var key = typeof(EventLogger).FullName;
            if (BuildEngine4.GetRegisteredTaskObject(key, RegisteredTaskObjectLifetime.Build) is EventLogger logger)
            {
                events = logger.Events;
                if (Dispose)
                {
                    BuildEngine4.UnregisterTaskObject(key, RegisteredTaskObjectLifetime.Build);
                    logger.Dispose();
                }
            }

            return true;
        }
    }
}