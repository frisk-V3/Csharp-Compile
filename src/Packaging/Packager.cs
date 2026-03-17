using System.IO.Compression;
using System.Runtime.InteropServices;

namespace csharpcompile.Packaging;

public static class Packager
{
    public static void Create(string sourceDir, string outputFile)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CreateZip(sourceDir, outputFile + ".zip");
        }
        else
        {
            CreateTarGz(sourceDir, outputFile + ".tar.gz");
        }
    }

    private static void CreateZip(string sourceDir, string zipFile)
    {
        if (File.Exists(zipFile))
            File.Delete(zipFile);

        ZipFile.CreateFromDirectory(sourceDir, zipFile);
        Console.WriteLine($"[PACKAGE] {zipFile}");
    }

    private static void CreateTarGz(string sourceDir, string tarFile)
    {
        // tarコマンド使う（クロス簡単）
        var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "tar",
            Arguments = $"-czf {tarFile} -C {Path.GetDirectoryName(sourceDir)} {Path.GetFileName(sourceDir)}",
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });

        process!.WaitForExit();

        Console.WriteLine($"[PACKAGE] {tarFile}");
    }
}
