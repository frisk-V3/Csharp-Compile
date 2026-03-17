using csharpcompile.Core;

namespace csharpcompile.Platforms.Mac;

public static class MacAppBundle
{
    public static void Create(ProjectInfo info)
    {
        var baseDir = Path.Combine(
            info.ProjectPath,
            "bin",
            "Release",
            info.Framework
        );

        var ridDir = Directory.GetDirectories(baseDir)
            .FirstOrDefault(x => x.Contains("osx"));

        if (ridDir == null) return;

        var publishDir = Path.Combine(ridDir, "publish");

        var appDir = Path.Combine(publishDir, $"{info.Name}.app");
        var contents = Path.Combine(appDir, "Contents");
        var macos = Path.Combine(contents, "MacOS");

        Directory.CreateDirectory(macos);

        foreach (var file in Directory.GetFiles(publishDir))
        {
            File.Copy(file, Path.Combine(macos, Path.GetFileName(file)), true);
        }

        File.WriteAllText(Path.Combine(contents, "Info.plist"), $@"
<?xml version=""1.0"" encoding=""UTF-8""?>
<plist version=""1.0"">
<dict>
    <key>CFBundleName</key>
    <string>{info.Name}</string>
    <key>CFBundleExecutable</key>
    <string>{info.Name}</string>
</dict>
</plist>");
    }
}
