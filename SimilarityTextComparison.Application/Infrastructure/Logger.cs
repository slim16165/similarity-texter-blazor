namespace SimilarityTextComparison.Application.Infrastructure;

public static class Logger
{
    public static void Info(string message)
    {
        Console.WriteLine($"[INFO] {DateTime.Now:O} - {message}");
    }

    public static void Error(string message, Exception ex = null)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now:O} - {message}");
        if (ex != null)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void Debug(string message)
    {
        Console.WriteLine($"[DEBUG] {DateTime.Now:O} - {message}");
    }
}