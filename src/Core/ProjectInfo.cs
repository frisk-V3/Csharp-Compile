using System.Xml.Linq;

namespace csharpcompile.Core;

public class ProjectInfo
{
    public string Name { get; set; } = "";
    public string Framework { get; set; } = "net8.0";
    public string ProjectPath { get; set; } = "";

    public static ProjectInfo Load(string path)
    {
        var csproj = Directory.Exists(path)
            ? Directory.GetFiles(path, "*.csproj").FirstOrDefault()
            : path;

        if (csproj == null)
            throw new Exception("No .csproj found");

        var doc = XDocument.Load(csproj);

        var ns = doc.Root?.Name.Namespace;

        var name =
            doc.Descendants(ns + "AssemblyName").FirstOrDefault()?.Value ??
            Path.GetFileNameWithoutExtension(csproj);

        var framework =
            doc.Descendants(ns + "TargetFramework").FirstOrDefault()?.Value ??
            "net8.0";

        return new ProjectInfo
        {
            Name = name,
            Framework = framework,
            ProjectPath = Path.GetDirectoryName(csproj)!
        };
    }
}
