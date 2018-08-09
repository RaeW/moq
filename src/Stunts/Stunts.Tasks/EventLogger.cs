using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Stunts.Tasks
{
    internal class EventLogger : ILogger, IDisposable
    {
        IEventSource source;

        public IList<BuildEventArgs> Events { get; } = new List<BuildEventArgs>();

        public string Parameters { get; set; }

        public LoggerVerbosity Verbosity { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.AnyEventRaised += OnEvent;
            source = eventSource;
        }

        void OnEvent(object sender, BuildEventArgs e) => Events.Add(e);

        public void Dispose() => source.AnyEventRaised -= OnEvent;

        public void Shutdown() { }
    }
}
