using System;
using System.Collections.Generic;
using System.IO;
using System.Json;

namespace Sunburst.DebianPackaging
{
    public sealed class PackagingConfiguration
    {
        public static PackagingConfiguration FromJsonFile(string jsonFilePath)
        {
            string content = File.ReadAllText(jsonFilePath);
            return FromJson((JsonObject)JsonValue.Parse(content));
        }

        public static PackagingConfiguration FromJsonStream(Stream stream)
        {
            return FromJson((JsonObject)JsonValue.Load(stream));
        }

        public static PackagingConfiguration FromJson(JsonObject json)
        {
            PackagingConfiguration config = new PackagingConfiguration();

            config.MaintainerName = json.TryGetValue("maintainer_name", "");
            config.MaintainerEmail = json.TryGetValue("maintainer_email", "");
            config.PackageName = json.TryGetValue("package_name", "");
            config.ShortDescription = json.TryGetValue("short_description", "");
            config.LongDescription = json.TryGetValue("long_description", "");
            config.Homepage = json.TryGetValue("homepage", "");

            JsonObject releaseObject = (JsonObject)json.TryGetValue("release", new JsonObject());
            config.PackageVersion = releaseObject.TryGetValue("package_version", "");
            config.PackageRevision = releaseObject.TryGetValue("package_revision", "1");
            config.ChangelogUrgency = Utilities.ParseEnum<ChangelogUrgency>(releaseObject.TryGetValue("urgency", "low"));
            config.ChangelogMessage = releaseObject.TryGetValue("changelog_message", "");

            JsonObject controlObject = (JsonObject)json.TryGetValue("control", new JsonObject());
            config.Priority = Utilities.ParseEnum<PackagePriority>(controlObject.TryGetValue("priority", "optional"));
            config.Section = Utilities.ParseEnum<PackageSection>(((string)controlObject.TryGetValue("section", "misc")).Replace('-', '_'));
            config.Architecture = controlObject.TryGetValue("architecture", "all");

            JsonObject dependencies = (JsonObject)json.TryGetValue("debian_dependencies", new JsonObject());
            foreach (var pair in dependencies)
            {
                PackageDependency depObject = new PackageDependency();
                depObject.Name = pair.Key;
                depObject.Version = ((JsonObject)pair.Value).TryGetValue("package_version", "");
                config.Dependencies.Add(depObject);
            }

            JsonObject symlinks = (JsonObject)json.TryGetValue("symlinks", new JsonObject());
            foreach (var pair in symlinks)
            {
                config.SymbolicLinks[pair.Key] = pair.Value;
            }

            return config;
        }

        public string MaintainerName { get; set; } = null;
        public string MaintainerEmail { get; set; } = null;
        public string PackageName { get; set; } = null;
        public string ShortDescription { get; set; } = null;
        public string LongDescription { get; set; } = null;
        public string Homepage { get; set; } = null;
        public string PackageVersion { get; set; } = null;
        public string PackageRevision { get; set; } = "1";
        public ChangelogUrgency ChangelogUrgency { get; set; } = ChangelogUrgency.Low;
        public string ChangelogMessage { get; set; } = null;
        public PackagePriority Priority { get; set; } = PackagePriority.Optional;
        public PackageSection Section { get; set; } = PackageSection.misc;
        public string Architecture { get; set; } = "all";
        public string Copyright { get; set; } = null;
        public string LicenseType { get; set; } = null;
        public string LicenseText { get; set; } = null;
        public HashSet<PackageDependency> Dependencies { get; } = new HashSet<PackageDependency>();
        public IDictionary<string, string> SymbolicLinks { get; } = new Dictionary<string, string>();
    }

    public struct PackageDependency
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PackageDependency other)
            {
                return Name == other.Name && Version == other.Version;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Version.GetHashCode();
        }
    }

    public enum ChangelogUrgency
    {
        Low,
        Medium,
        High,
        Emergency,
        Critical
    }

    public enum PackagePriority
    {
        Required,
        Important,
        Standard,
        Optional
    }

    public enum PackageSection
    {
        admin,
        cli_mono,
        comm,
        database,
        debug,
        devel,
        doc,
        editors,
        education,
        electronics,
        embedded,
        fonts,
        games,
        gnome,
        gnu_r,
        gnustep,
        graphics,
        hamradio,
        haskell,
        httpd,
        interpreters,
        introspection,
        java,
        javascript,
        kde,
        kernel,
        libdevel,
        libs,
        lisp,
        localization,
        mail,
        math,
        metapackages,
        misc,
        net,
        news,
        ocaml,
        oldlibs,
        otherosfs,
        perl,
        php,
        python,
        ruby,
        rust,
        science,
        shells,
        sound,
        tasks,
        tex,
        text,
        utils,
        vcs,
        video,
        web,
        x11,
        xfce,
        zope
    }
}
