using System.Diagnostics;

namespace csharpcompile.Core;

public static class Builder
{
    public static void Publish(string projectPath, string target)
    {
        Console.WriteLine($"[INFO] Publishing {projectPath} -> {target}");

        var args = $"publish \"{projectPath}\" -c Release -r {target} --self-contained true";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        var process = Process.Start(psi);

        process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.WriteLine("[ERROR] Build failed");
            Environment.Exit(1);
        }

        Console.WriteLine("[SUCCESS] Build complete");

        // Macだけ特別処理
        if (target.StartsWith("osx"))
        {
            Platforms.Mac.MacAppBundle.Create(projectPath, target);
        }
    }
}
