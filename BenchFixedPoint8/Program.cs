namespace Gitan.FixedPoint8;

public class Program
{
    public static void Main()
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Calc>();
        //BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Characters>();
        //BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Utf8Json>();
    }
}