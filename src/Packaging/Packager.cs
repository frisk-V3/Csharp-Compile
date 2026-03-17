namespace csharpcompile.Packaging;

public static class Packager
{
    public static void Zip(string path)
    {
        Console.WriteLine($"[INFO] Packaging {path}");
        // 将来: zip / tar.gz 対応
    }
}
