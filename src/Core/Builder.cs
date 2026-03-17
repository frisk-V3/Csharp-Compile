using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace csharpcompile.Core;

public static class Builder
{
    public static void PublishMulti(string projectPath, string targets, string output, bool aot)
    {
        var targetList = targets.Split(',', StringSplitOptions.RemoveEmptyEntries);
        Directory.CreateDirectory(output);

        foreach (var target in targetList)
        {
            PublishSingle(projectPath, target.Trim(), output, aot);
        }
    }

    private static void PublishSingle(string projectPath, string target, string output, bool aot)
    {
        Console.WriteLine($"[INFO] Building {target}");

        var aotArg = aot ? "-p:PublishAot=true" : "";
        var args = $"publish \"{projectPath}\" -c Release -r {target} --self-contained true {aotArg}";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        var process = Process.Start(psi)!;
        process.OutputDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"[ERROR] Failed {target}");
            return;
        }

        var publishDir = Path.Combine(projectPath, "bin", "Release", "net8.0", target, "publish");
        var destDir = Path.Combine(output, $"{target}");
        if (Directory.Exists(destDir)) Directory.Delete(destDir, true);
        CopyDirectory(publishDir, destDir);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            foreach (var file in Directory.GetFiles(destDir))
                Process.Start("chmod", $"+x {file}")?.WaitForExit();
        }

        // macOS .app化（簡易サンプル）
        if (target.StartsWith("osx"))
        {
            var appDir = Path.Combine(destDir, "csharpcompile.app", "Contents", "MacOS");
            Directory.CreateDirectory(appDir);
            File.Copy(Path.Combine(destDir, "csharpcompile"), Path.Combine(appDir, "csharpcompile"), true);
        }

        Console.WriteLine($"[SUCCESS] {target}");
    }

    private static void CopyDirectory(string source, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (var file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);

        foreach (var dir in Directory.GetDirectories(source))
            CopyDirectory(dir, Path.Combine(dest, Path.GetFileName(dir)));
    }
}
