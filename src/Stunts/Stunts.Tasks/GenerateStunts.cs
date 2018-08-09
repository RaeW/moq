using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Xamarin.Build;

namespace Stunts.Tasks
{
    public class GenerateStunts : AsyncTask, IBuildConfiguration
    {
        [Required]
        public string ProjectFullPath { get; set; }

        /// <summary>
        /// Gets or sets the paths to directories to search for dependencies.
        /// </summary>
        [Required]
        public ITaskItem[] AssemblySearchPath { get; set; }

        [Required]
        public string MSBuildBinPath { get; set; }

        [Required]
        public string ToolsPath { get; set; }

        public bool BuildingInsideVisualStudio { get; set; }

        /// <summary>
        /// Whether to debug debug the task for troubleshooting purposes.
        /// </summary>
        public bool DebugTask { get; set; }

        /// <summary>
        /// Whether to cause the console program to launch a debugger on run 
        /// for troubleshooting purposes.
        /// </summary>
        public bool DebugConsole { get; set; }

        IDictionary<string, string> IBuildConfiguration.GlobalProperties => BuildEngine.GetGlobalProperties();

        public override bool Execute()
        {
            using (var resolver = new AssemblyResolver(AssemblySearchPath, (i, m) => Log.LogMessage(i, m)))
            {
                Task.Run(ExecuteAsync).ConfigureAwait(false);
                return base.Execute();
            }
        }

        private async Task ExecuteAsync()
        {
            if (DebugTask)
                Debugger.Launch();
            
            var solutionConfig = new Dictionary<string, (string configuration, string platform)>();
            //if (!string.IsNullOrEmpty(SolutionConfiguration))
            //{
            //    solutionConfig = XElement.Parse(SolutionConfiguration)
            //        .Descendants("ProjectConfiguration")
            //        .Select(x => new { AbsolutePath = x.Attribute("AbsolutePath").Value, Configuration = x.Value })
            //        .ToDictionary(x => x.AbsolutePath, x =>
            //        {
            //            // Debug|AnyCPU
            //            var configPlat = x.Configuration.Split('|');
            //            return (configPlat[0], configPlat[1]);
            //        }, StringComparer.OrdinalIgnoreCase);
            //}

            var watch = Stopwatch.StartNew();
            var workspace = this.GetWorkspace();
            var project = await workspace.GetOrAddProjectAsync(ProjectFullPath, solutionConfig, (i, m) => LogMessage(m, i), Token);
            watch.Stop();

            LogMessage($"Loaded {project.Name} in {watch.Elapsed.TotalSeconds} seconds");

            Complete();
        }
    }
}
