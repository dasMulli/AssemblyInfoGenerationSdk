using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;
using Microsoft.Build.Locator;

namespace DasMulli.AssemblyInfoGeneration.Tests
{
    public class GivenWeCareAboutAssemblyAttributes
    {
        [Theory]
        [InlineData(TestAsset.ConsoleApp)]
        [InlineData(TestAsset.WebApplication)]
        [InlineData(TestAsset.WindowsFormsApp)]
        [InlineData(TestAsset.WpfApp)]
        public void ItShallCreateAssemblyAttributes(TestAsset asset)
        {
            var assets = TestAssetsManager.CloneAssetsForTest(asset);
            try
            {
                var startInfo = CreateMsBuildInvocation(assets, "-p:Version=1.2.3-beta.005 -p:FileVersion=1.2.3.4 -p:AssemblyVersion=1.2.3.0");
                var process = Process.Start(startInfo);
                Assert.NotNull(process);
                process.WaitForExit(30_000).Should().BeTrue();

                var builtOutput = new FileInfo(Path.Combine(assets.FullName, asset.GetExpectedOutputAssembly()));
                builtOutput.Exists.Should().BeTrue();

                var versionInfo = FileVersionInfo.GetVersionInfo(builtOutput.FullName);
                versionInfo.Should().NotBeNull();
                versionInfo.ProductVersion.Should().Be("1.2.3-beta.005");
                versionInfo.FileVersion.Should().Be("1.2.3.4");

                Assembly.ReflectionOnlyLoadFrom(builtOutput.FullName).GetName().Version.Should().BeEquivalentTo(new Version("1.2.3.0"));
            }
            finally
            {
                assets.Delete(true);
            }
        }

        private static ProcessStartInfo CreateMsBuildInvocation(DirectoryInfo assets, string arguments)
        {
            var msbuild = ToolLocator.LocateMSBuildExecutable();
            return new ProcessStartInfo
            {
                EnvironmentVariables = { { "MSBuildSdkPath", Path.Combine(AppContext.BaseDirectory, "Sdks") } },
                FileName = msbuild,
                Arguments = "-nr:false " + arguments,
                WorkingDirectory = assets.FullName,
                UseShellExecute = false
            };
        }
    }
}
