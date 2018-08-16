using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Build.Locator;

namespace DasMulli.AssemblyInfoGeneration.Tests
{
    public static class ToolLocator
    {
        private static string _cachedMsBuildLocation;

        public static string LocateMSBuildExecutable()
        {
            if (_cachedMsBuildLocation == null)
            {
                var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToList();
                visualStudioInstances.Should().NotBeEmpty("Visual Studio installation required");

                visualStudioInstances.Sort((instanceA, instanceB) => instanceB.Version.CompareTo(instanceA.Version));

                var latestVS = visualStudioInstances[0];

                var msbuildPath = Path.Combine(latestVS.MSBuildPath, "MSBuild.exe");

                _cachedMsBuildLocation = msbuildPath;
                return msbuildPath;
            }

            return _cachedMsBuildLocation;
        }
    }
}
