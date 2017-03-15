using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Sunburst.Json;

namespace Sunburst.DebianPackaging
{
    internal sealed class ConfigurationGenerator
    {
        private static string FormatChangelogDate(DateTime date)
        {
            DateTime utcDate = date.ToUniversalTime();
            StringBuilder formattedDate = new StringBuilder();

            switch (utcDate.DayOfWeek)
            {
                case DayOfWeek.Sunday: formattedDate.Append("Sun"); break;
                case DayOfWeek.Monday: formattedDate.Append("Mon"); break;
                case DayOfWeek.Tuesday: formattedDate.Append("Tue"); break;
                case DayOfWeek.Wednesday: formattedDate.Append("Wed"); break;
                case DayOfWeek.Thursday: formattedDate.Append("Thu"); break;
                case DayOfWeek.Friday: formattedDate.Append("Fri"); break;
                case DayOfWeek.Saturday: formattedDate.Append("Sat"); break;
            }
            formattedDate.Append(" ");

            formattedDate.Append(utcDate.Day.ToString("D2"));
            formattedDate.Append(" ");

            switch (utcDate.Month)
            {
                case 1: formattedDate.Append("Jan"); break;
                case 2: formattedDate.Append("Feb"); break;
                case 3: formattedDate.Append("Mar");break;
                case 4: formattedDate.Append("Apr");break;
                case 5: formattedDate.Append("May");break;
                case 6: formattedDate.Append("Jun");break;
                case 7: formattedDate.Append("Jul");break;
                case 8: formattedDate.Append("Aug");break;
                case 9: formattedDate.Append("Sep");break;
                case 10: formattedDate.Append("Oct");break;
                case 11: formattedDate.Append("Nov");break;
                case 12: formattedDate.Append("Dec");break;
                default: throw new ArgumentException("Unrecognized month: this can't happen", nameof(date));
            }

            formattedDate.Append(" ");
            formattedDate.Append(utcDate.Year);
            formattedDate.Append(" ");

            formattedDate.Append(utcDate.Hour.ToString("D2"));
            formattedDate.Append(":");
            formattedDate.Append(utcDate.Minute.ToString("D2"));
            formattedDate.Append(":");
            formattedDate.Append(utcDate.Millisecond.ToString("D2"));
            formattedDate.Append(" +0000");

            return formattedDate.ToString();
        }

        private static string FormatDependentPackageString(JsonDictionary dependencyData)
        {
            if (dependencyData == null) return string.Empty;

            List<string> dependencies = new List<string>();
            foreach (KeyValuePair<string, JsonObject> pair in dependencyData)
            {
                string dep_str = pair.Key;

                JsonDictionary value = (JsonDictionary)pair.Value;
                JsonObject dependencyVersionObj = value.TryGetValue("package_version", null);
                if (dependencyVersionObj != null)
                {
                    dep_str += $" (>= {((JsonString)dependencyVersionObj).Value})";
                }

                dependencies.Add(dep_str);
            }

            // The leading comma is important here.
            return ", " + string.Join(", ", dependencies);
        }

        public ConfigurationGenerator(JsonDictionary config_data)
        {
            ConfigurationData = config_data;
            MyAssembly = typeof(ConfigurationGenerator).Assembly;
        }

        public void Generate(DirectoryInfo outputDirectory, string nameOverride = null, string versionOverride = null)
        {
            GenerateChangeLog(outputDirectory.GetFile("changelog"), nameOverride: nameOverride, versionOverride: versionOverride);
            GeneratePackageControl(outputDirectory.GetFile("control"), nameOverride: nameOverride);
            GenerateCopyright(outputDirectory.GetFile("copyright"));
            GenerateRulesFile(outputDirectory.GetFile("rules"));

            string packageName = nameOverride ?? GetConfigString("package_name");
            string symlinkFileName = packageName + ".links";
            GenerateSymlinksFile(outputDirectory.GetFile(symlinkFileName), nameOverride: nameOverride);
        }

        private readonly JsonDictionary ConfigurationData;
        private readonly Assembly MyAssembly;

        private string GetConfigString(string key) => GetConfigString(key, null) ?? throw new KeyNotFoundException(key);
        private string GetConfigString(string key, string defaultValue) => GetJsonStringValue(ConfigurationData, key, defaultValue);
        private string GetJsonStringValue(JsonDictionary dict, string key) => GetJsonStringValue(dict, key, null) ?? throw new KeyNotFoundException(key);

        private string GetJsonStringValue(JsonDictionary dict, string key, string defaultValue)
        {
            JsonObject obj = dict[key];
            if (obj == null) return defaultValue;
            else return ((JsonString)obj).Value;
        }

        private string GetTemplate(string templateName)
        {
            using (StreamReader reader = new StreamReader(MyAssembly.GetManifestResourceStream(templateName)))
            {
                return reader.ReadToEnd();
            }
        }

        private void GenerateRulesFile(FileInfo file)
        {
            JsonArray ignoredDependencies = (JsonArray)ConfigurationData.TryGetValue("debian_ignored_dependencies", null);
            StringBuilder overrideText = new StringBuilder();

            if (ignoredDependencies != null)
            {
                overrideText.Append("override_dh_shlibdeps:\n");
                overrideText.Append("\tdh_shlibdeps --dpkg-shlibdeps-params=\"");

                foreach (JsonObject obj in ignoredDependencies)
                {
                    JsonString depName = (JsonString)obj;
                    overrideText.AppendFormat("-x{0} ", depName.Value);
                }

                overrideText.Append("\"\n");
            }

            using (var writer = new StreamWriter(file.Open(FileMode.Create)))
            {
                string data = GetTemplate("rules.tpl").Format(("overrides", overrideText.ToString()));
                writer.Write(data);
            }
        }

        private void GenerateChangeLog(FileInfo file, string versionOverride = null, string nameOverride = null)
        {
            JsonDictionary releaseData = (JsonDictionary)ConfigurationData["release"];

            var templateParameters = new(string, string)[]
            {
                ("PACKAGE_VERSION", versionOverride ?? GetJsonStringValue(releaseData, "package_version")),
                ("PACKAGE_REVISION", GetJsonStringValue(releaseData, "package_revision")),
                ("CHANGELOG_MESSAGE", GetJsonStringValue(releaseData, "changelog_message")),
                ("URGENCY", GetJsonStringValue(releaseData, "urgency", "low")),

                ("PACKAGE_NAME", nameOverride ?? GetConfigString("package_name")),
                ("MAINTAINER_NAME", GetConfigString("maintainer_name")),
                ("MAINTAINER_EMAIL", GetConfigString("maintainer_email")),
                ("DATE", FormatChangelogDate(DateTime.Now))
            };

            using (var writer = new StreamWriter(file.Open(FileMode.Create)))
            {
                string data = GetTemplate("changelog.tpl").Format(templateParameters);
                writer.Write(data);
            }
        }

        private void GeneratePackageControl(FileInfo file, string nameOverride = null)
        {
            JsonDictionary depData = (JsonDictionary)ConfigurationData.TryGetValue("debian_dependencies", null);
            string depStr = FormatDependentPackageString(depData);

            string packageConflicts = string.Join(", ", ((JsonArray)ConfigurationData["package_conflicts"]).Select(x => ((JsonString)x).Value));

            var templateParameters = new(string, string)[]
            {
                ("SHORT_DESCRIPTION", GetConfigString("short_description")),
                ("LONG_DESCRIPTION", GetConfigString("long_description")),
                ("HOMEPAGE", GetConfigString("homepage", "")),

                ("SECTION", GetConfigString("section", "misc")),
                ("PRIORITY", GetConfigString("priority", "low")),
                ("ARCH", GetConfigString("architecture", "all")),

                ("DEPENDENT_PACKAGES", depStr),
                ("CONFLICT_PACKAGES", packageConflicts),

                ("PACKAGE_NAME", nameOverride ?? GetConfigString("package_name")),
                ("MAINTAINER_NAME", GetConfigString("maintainer_name")),
                ("MAINTAINER_EMAIL", GetConfigString("maintainer_email"))
            };

            using (var writer = new StreamWriter(file.Open(FileMode.Create)))
            {
                string data = GetTemplate("control.tpl").Format(templateParameters);
                writer.Write(data);
            }
        }

        private void GenerateCopyright(FileInfo file)
        {
            JsonDictionary licenseData = (JsonDictionary)ConfigurationData["license"];

            var templateParameters = new(string, string)[]
            {
                ("COPYRIGHT_TEXT", GetConfigString("copyright")),
                ("LICENSE_NAME", GetJsonStringValue(licenseData, "type")),
                ("LICENSE_TEXT", GetJsonStringValue(licenseData, "text"))
            };

            using (var writer = new StreamWriter(file.Open(FileMode.Create)))
            {
                string data = GetTemplate("copyright.tpl").Format(templateParameters);
                writer.Write(data);
            }
        }

        private string GetPackageRootDirectory(string nameOverride = null)
        {
            string configRoot = GetConfigString("install_root", null);
            string defaultRoot = "/usr/share/" + (nameOverride ?? GetConfigString("package_name"));
            return configRoot ?? defaultRoot;
        }

        private void GenerateSymlinksFile(FileInfo file, string nameOverride = null)
        {
            JsonDictionary symlinkData = (JsonDictionary)ConfigurationData.TryGetValue("symlinks", null);
            if (symlinkData != null)
            {
                string packageRoot = GetPackageRootDirectory(nameOverride);
                List<string> symlinkEntries = new List<string>();

                foreach (var pair in symlinkData)
                {
                    string packageAbsPath = packageRoot + "/" + pair.Key;
                    symlinkEntries.Add($"{packageAbsPath} {pair.Value}");
                }

                string data = string.Join("\n", symlinkEntries);
                using (var writer = new StreamWriter(file.Open(FileMode.Create)))
                {
                    writer.Write(data);
                }
            }
        }
    }
}
