using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace DasMulli.AssemblyInfoGeneration.Tests
{
    internal static class TestAssetsManager
    {
        private static readonly string TestAssetsBasePath = Path.Combine(AppContext.BaseDirectory, "TestAssets");

        public static DirectoryInfo CloneAssetsForTest(TestAsset asset, [CallerMemberName] string callerMemberName = default)
        {
            if (!Enum.IsDefined(typeof(TestAsset), asset))
            {
                throw new InvalidEnumArgumentException(nameof(asset), (int) asset, typeof(TestAsset));
            }

            if (callerMemberName == null)
            {
                callerMemberName = Guid.NewGuid().ToString();
            }

            var testAssetDirectoryName = asset.ToString();

            var sourceDir = new DirectoryInfo(Path.Combine(TestAssetsBasePath, testAssetDirectoryName));
            var targetDir = new DirectoryInfo(Path.Combine(TestAssetsBasePath, $"{testAssetDirectoryName}_{callerMemberName}"));

            CopyDirectory(sourceDir, targetDir);

            return targetDir;
        }
        
        private static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

    }
}