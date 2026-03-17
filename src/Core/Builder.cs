using System.Diagnostics;

namespace csharpcompile.Core;

public static class Builder
{
    public static void PublishMulti(string projectPath, string targets, string output, bool aot)
    {
        var info = ProjectInfo.Load(projectPath);

        var targetList = targets.Split(',', StringSplitOptions.RemoveEmptyEntries);

        Directory.CreateDirectory(output);

        foreach (var target in targetList)
        {
            PublishSingle(info, target.Trim(), output, aot);
        }

        Console.WriteLine("[SUCCESS] All builds completed");
    }

    private static void PublishSingle(ProjectInfo info, string target, string output, bool aot)
    {
        Console.WriteLine($"\n[INFO] Building {target}");

        var aotArg = aot ? "-p:PublishAot=true" : "";

        var args =
            $"publish \"{info.ProjectPath}\" -c Release -r {target} --self-contained true {aotArg}";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        var process = Process.Start(psi)!;

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null) Console.WriteLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null) Console.WriteLine(e.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"[ERROR] Failed: {target}");
            return;
        }

        var publishDir = Path.Combine(
            info.ProjectPath,
            "bin",
            "Release",
            info.Framework,
            target,
            "publish"
        );

        var destDir = Path.Combine(output, $"{info.Name}-{target}");

        if (Directory.Exists(destDir))
            Directory.Delete(destDir, true);

        DirectoryCopy(publishDir, destDir);

        if (target.StartsWith("osx"))
        {
            Platforms.Mac.MacAppBundle.Create(info);
        }

        Console.WriteLine($"[SUCCESS] {target} -> {destDir}");
    }

    private static void DirectoryCopy(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var newDir = Path.Combine(destDir, Path.GetFileName(dir));
            DirectoryCopy(dir, newDir);
        }
    }
}
