﻿using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Stunts.Tasks
{
    internal interface IBuildConfiguration
    {
        IBuildEngine4 BuildEngine4 { get; }

        bool BuildingInsideVisualStudio { get; }

        IDictionary<string, string> GlobalProperties { get; }

        string MSBuildBinPath { get; }

        string ToolsPath { get; }
    }
}
