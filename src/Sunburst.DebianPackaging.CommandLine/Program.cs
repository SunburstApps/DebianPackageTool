using System;
using System.CommandLine;
using System.IO;

namespace Sunburst.DebianPackaging.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = Directory.GetCurrentDirectory();
            string outputDir = Path.Combine(inputDir, "bin", "dpkg");
            string overrideName = null, overrideVersion = null;

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.ApplicationName = "dotnet deb-tool";
                syntax.DefineOption("i|input-dir", ref inputDir, "The path to the package source directory (default: current directory)");
                syntax.DefineOption("o|output-dir", ref outputDir, "Where to create the final .deb files (default ./bin/dpkg)");
                syntax.DefineOption("override-name", ref overrideName, "Override the package name in the package_config.json file");
                syntax.DefineOption("override-version", ref overrideVersion, "Override the package version in the package_config.json file");
            });
        }
    }
}
