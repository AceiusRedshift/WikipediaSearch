using System.Diagnostics;
namespace Indexer;

static class StopwatchExtensions
{
    public static void Report(this Stopwatch timer, string text)
    {
        Console.WriteLine($"{text} in {timer.Elapsed}");
        timer.Restart();
    }
}