/// <summary>
/// To run this in Visual Studio:
/// 1. Set build configure as Release.
/// 2. Debug menu, Start without debugging.
/// 3. After done, check "Source\GobangBenchMark\bin\Release\BenchmarkDotNet.Artifacts\results\BoardBenchMark-report.html"
/// </summary>
namespace GobangBenchMark
{
    class Program
    {
        static void Main(string[] args)
        {
            //new BoardEmptyPositionIterationBenchMark().Run();
            // new MinmaxSearchAiBenchMark().Run();
            new AbPruningAiBenchMark().Run();
        }
    }
}
