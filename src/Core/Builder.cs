using System.Diagnostics;

namespace csharpcompile.Core;

public static class Builder
{
    public static void Publish(string projectPath, string target)
    {
        var info = ProjectInfo.Load(projectPath);

        Console.WriteLine($"[INFO] Project: {info.Name}");
        Console.WriteLine($"[INFO] Framework: {info.Framework}");
        Console.WriteLine($"[INFO] Target: {target}");

        var args = $"publish \"{info.ProjectPath}\" -c Release -r {target} --self-contained true";

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
            Console.WriteLine("[ERROR] Build failed");
            Environment.Exit(1);
        }

        Console.WriteLine("[SUCCESS] Build complete");

        if (target.StartsWith("osx"))
        {
            Platforms.Mac.MacAppBundle.Create(info);
        }
    }
}
