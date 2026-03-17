using csharpcompile.Core;

namespace csharpcompile.Platforms.Mac;

public static class MacAppBundle
{
    public static void Create(ProjectInfo info)
    {
        Console.WriteLine("[INFO] Creating .app bundle...");

        var publishRoot = Path.Combine(
            info.ProjectPath,
            "bin",
            "Release",
            info.Framework
        );

        var ridDir = Directory.GetDirectories(publishRoot).FirstOrDefault();

        if (ridDir == null)
        {
            Console.WriteLine("[WARN] RID directory not found");
            return;
        }

        var publishDir = Path.Combine(ridDir, "publish");

        var appDir = Path.Combine(publishDir, $"{info.Name}.app");
        var contents = Path.Combine(appDir, "Contents");
        var macos = Path.Combine(contents, "MacOS");

        Directory.CreateDirectory(macos);

        foreach (var file in Directory.GetFiles(publishDir))
        {
            var dest = Path.Combine(macos, Path.GetFileName(file));
            File.Copy(file, dest, true);
        }

        var plist = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"">
<plist version=""1.0"">
<dict>
    <key>CFBundleName</key>
    <string>{info.Name}</string>
    <key>CFBundleExecutable</key>
    <string>{info.Name}</string>
</dict>
</plist>";

        File.WriteAllText(Path.Combine(contents, "Info.plist"), plist);

        Console.WriteLine($"[SUCCESS] {info.Name}.app created");
    }
}
