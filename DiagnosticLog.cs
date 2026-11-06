using System.Diagnostics;

namespace MauiAboutBlankRepro;

public static class DiagnosticLog
{
    private static readonly object Gate = new();
    private static readonly string PathToLog = Path.Combine(FileSystem.AppDataDirectory, "about-repro-diagnostics.txt");
    private static long _sequence;

    public static string FilePath => PathToLog;

    public static void Write(string message)
    {
        lock (Gate)
        {
            var line = $"{DateTimeOffset.Now:O} #{++_sequence} {message}";
            Debug.WriteLine($"[AboutRepro] {line}");
            try
            {
                File.AppendAllText(PathToLog, line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AboutRepro] Could not write diagnostics: {ex}");
            }
        }
    }
}
