<<<<<<< HEAD
﻿using BenchmarkDotNet.Running;

namespace Gitan.FixedPoint8;

public class Program
{
    //static void Main(string[] args)
    //{
    //    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    //}

    public static void Main()
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Calc>();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Characters>();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Utf8Json>();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_TryFormat>();
    }
=======
﻿using BenchmarkDotNet.Running;

namespace Gitan.FixedPoint8;

public class Program
{
    //static void Main(string[] args)
    //{
    //    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    //}

    public static void Main()
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Calc>();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Characters>();
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchMark_Utf8Json>();
    }
>>>>>>> 1c034ee14a2fed2c550bfe840c3700753e00637a
}