# AssemblyInfoGenerationSdk

Generates assembly attributes ("AssemblyInfo.cs") based on MSBuild properties for classic .NET project types (classic ASP.NET, WPF, WinForms, Console, ...) similar to .NET Core and .NET Standard project.

It containes isolated copies of the `Microsoft.NET.Sdk` build logic and uses MSBuild task dependency isolation logic by @AArnott to do this in a compatible and non-conflicting way.

# Usage

Use this in your favourite project by [referencing it as MSBuild Project SDK](https://docs.microsoft.com/en-us/visualstudio/msbuild/how-to-use-project-sdk) from your project file (`.csproj` / `.vbproj`):

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Sdk Name="DasMulli.AssemblyInfoGeneration.Sdk" Version="1.0.0-beta001" />
…
```