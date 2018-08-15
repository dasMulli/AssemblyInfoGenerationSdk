using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DasMulli.AssemblyInfoGeneration.Sdk
{
    /// <inheritdoc />
    /// <summary>
    /// Determines the assembly version to use for a given semantic version.
    /// </summary>
    public class GetAssemblyVersion : ContextAwareTask
    {
        /// <summary>
        /// The nuget version from which to get an assembly version portion.
        /// </summary>
        [Required]
        public string NuGetVersion { get; set; }

        /// <summary>
        /// The assembly version (major.minor.patch.revision) portion of the nuget version.
        /// </summary>
        [Output]
        public string AssemblyVersion { get; set; }

        protected override bool ExecuteInner()
        {
            if (!NuGet.Versioning.NuGetVersion.TryParse(NuGetVersion, out var nugetVersion))
            {
                Log.LogError(
                    subcategory: default,
                    errorCode: "NETSDK1018",
                    helpKeyword: default,
                    file: default,
                    lineNumber: default,
                    columnNumber: default,
                    endLineNumber: default,
                    endColumnNumber: default,
                    message: $"Invalid NuGet version string: '{NuGetVersion}'");
                return false;
            }

            AssemblyVersion = nugetVersion.Version.ToString();
            return true;
        }
    }
}
