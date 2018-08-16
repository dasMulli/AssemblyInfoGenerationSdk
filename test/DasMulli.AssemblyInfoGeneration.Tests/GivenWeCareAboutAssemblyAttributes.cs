using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DasMulli.AssemblyInfoGeneration.Tests.Utils;
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
            var (assets, projectFile) = TestAssetsManager.CloneAssetsForTest(asset);
            try
            {
                RemoveSdkVersionFromProjectFile(projectFile).Should().BeTrue();

                var startInfo = CreateMsBuildInvocation(assets, $"-p:Version=1.2.3-beta.005 -p:FileVersion=1.2.3.4 -p:AssemblyVersion=1.2.3.0 -bl:..\\{nameof(ItShallCreateAssemblyAttributes)}_{asset}.binlog");
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
            var startInfo = new ProcessStartInfo
            {
                FileName = msbuild,
                Arguments = "-nr:false -m:1 " + arguments,
                WorkingDirectory = assets.FullName,
                UseShellExecute = false
            };
            startInfo.EnvironmentVariables["MSBuildSDKsPath"] = Path.Combine(AppContext.BaseDirectory, "Sdks");
            startInfo.EnvironmentVariables["Configuration"] = "Release";
            return startInfo;
        }

        private static bool RemoveSdkVersionFromProjectFile(FileInfo projectFile) => new XmlPoke(projectFile.FullName, "/x:Project/x:Sdk[@Name=\"DasMulli.AssemblyInfoGeneration.Sdk\"]/@Version", "",
                "<Namespace Prefix='x' Uri='http://schemas.microsoft.com/developer/msbuild/2003' />")
            .Execute();
    }
}
