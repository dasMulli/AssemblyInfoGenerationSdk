using System;
using System.ComponentModel;
using System.IO;

namespace DasMulli.AssemblyInfoGeneration.Tests
{
    internal static class TestAssetExtensions
    {
        public static string GetExpectedOutputAssembly(this TestAsset asset)
        {
            if (!Enum.IsDefined(typeof(TestAsset), asset))
            {
                throw new InvalidEnumArgumentException(nameof(asset), (int) asset, typeof(TestAsset));
            }

            return asset == TestAsset.WebApplication
                ? Path.Combine("bin", asset + ".dll")
                : Path.Combine("bin", "Debug", asset + ".exe");
        }
    }
}