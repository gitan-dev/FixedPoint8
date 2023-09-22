
using BenchmarkDotNet.Attributes;

namespace Gitan.FixedPoint8;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<保留中>")]

public class BenchMark_Calc
{

    /////////////////////////////////////// ROS Parse

    static readonly FixedPoint8 fixedPoint8Value = FixedPoint8.FromInnerValue(-12_3400_0000);
    static readonly FixedPoint8 v2 = FixedPoint8.FromInnerValue(2_0000_0000);
    static readonly FixedPoint8 v10 = FixedPoint8.FromInnerValue(10_0000_0000);
    static readonly int intValue = -1234;
    static readonly double doubleValue = -12.34;
    static readonly decimal decimalValue = -12.34m;

    static readonly int[] _intValues = new int[]
    {
        1000000000,
        123456789,
        12345678,
        1234567,
        123456,
        1234,
        12,
        1,
        0,
        -1000000000,
        -123456789,
        -12345678,
        -1234567,
        -123456,
        -1234,
        -12,
        -1,
    };

    static readonly decimal[] _decimalValues = new decimal[]
    {
        12345678m,
        1234m,
        12m,
        1m,
        0m,
        0.1m,
        0.12m,
        0.1234m,
        0.12345678m,
        -12345678m,
        -1234m,
        -12m,
        -1m,
        -0.1m,
        -0.12m,
        -0.1234m,
        -0.12345678m,
    };

    static readonly double[] _doubleValues = _decimalValues.Select(x => (double)x).ToArray();
    static readonly FixedPoint8[] _fp8Values = _decimalValues.Select(x => (FixedPoint8)x).ToArray();

    /// ////////////////////////////////////// Kakeru 

    [Benchmark]
    public int Mul2Int()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _intValues)
            {
                sum += value * 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public double Mul2Double()
    {
        var sum = 0.0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _doubleValues)
            {
                sum += value * 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public decimal Mul2Decimal()
    {
        var sum = 0.0m;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _decimalValues)
            {
                sum += value * 2m;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 MulInt2FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach(var value in _fp8Values)
            {
                sum += value * 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Mul2FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value * v2;
            }
        }
        return sum;
    }


    [Benchmark]
    public int Mul10Int()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _intValues)
            {
                sum += intValue * 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public double Mul10Double()
    {
        var sum = 0.0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _doubleValues)
            {
                sum += value * 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public decimal Mul10Decimal()
    {
        var sum = 0.0m;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _decimalValues)
            {
                sum += value * 10m;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 MulInt10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value * 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Mul10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value * v10;
            }
        }
        return sum;
    }


    /// ////////////////////////////////////// Plus

    [Benchmark]
    public int Add2Int()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _intValues)
            {
                sum += value + 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public double Add2Double()
    {
        var sum = 0.0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _doubleValues)
            {
                sum += value + 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public decimal Add2Decimal()
    {
        var sum = 0.0m;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _decimalValues)
            {
                sum += value + 2m;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Add2FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value + v2;
            }
        }
        return sum;
    }

    [Benchmark]
    public int Add10Int()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _intValues)
            {
                sum += value + 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public double Add10Double()
    {
        var sum = 0.0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _doubleValues)
            {
                sum += value + 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public decimal Add10Decimal()
    {
        var sum = 0.0m;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _decimalValues)
            {
                sum += value + 10m;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Add10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value + v10;
            }
        }
        return sum;
    }


    ///// ////////////////////////////////////// Minus

    [Benchmark]
    public int Sub2Int()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _intValues)
            {
                sum += value - 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public double Sub2Double()
    {
        var sum = 0.0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _doubleValues)
            {
                sum += value - 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public decimal Sub2Decimal()
    {
        var sum = 0.0m;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _decimalValues)
            {
                sum += value - 2m;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Sub2FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value - v2;
            }
        }
        return sum;
    }

    [Benchmark]
    public int Sub10Int()
    {
        var sum = 0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _intValues)
            {
                sum += value - 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public double Sub10Double()
    {
        var sum = 0.0;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _doubleValues)
            {
                sum += value - 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public decimal Sub10Decimal()
    {
        var sum = 0.0m;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _decimalValues)
            {
                sum += value - 10m;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Sub10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value - v10;
            }
        }
        return sum;
    }


    /// ////////////////////////////////////// 大なり小なり

    [Benchmark]
    public bool LessThanInt()
    {
        var result = intValue < 10;
        return result;
    }

    [Benchmark]
    public bool LessThanDouble()
    {
        var result = doubleValue < 10;
        return result;
    }

    [Benchmark]
    public bool LessThanDecimal()
    {
        var result = decimalValue < 10m;
        return result;
    }

    [Benchmark]
    public bool LessThanFixedPoint8()
    {
        var result = fixedPoint8Value < v10;
        return result;
    }


// Math

    // Round
    [Benchmark]
    public FixedPoint8 FixedPoint8Round()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach(var value in _fp8Values)
        {
            sum += value.Round();
        }
        return sum;
    }

    [Benchmark]
    public decimal MathRoundDecimal()
    {
        decimal result = 0m;
        foreach (var value in _decimalValues)
        {
            result = Math.Round(value);
        }
        return result;
    }

    [Benchmark]
    public double MathRoundDouble()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Round(value);
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 FixedPoint8Round2()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Round(2);
        }
        return sum;
    }

    [Benchmark]
    public decimal MathRound2Decimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Round(value, 2);
        }
        return sum;
    }

    [Benchmark]
    public double MathRound2Double()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Round(value, 2);
        }
        return sum;
    }


    // Floor
    [Benchmark]
    public FixedPoint8 FixedPoint8Floor()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Floor();
        }
        return sum;
    }

    [Benchmark]
    public decimal MathFloorDecimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Floor(value);
        }
        return sum;
    }

    [Benchmark]
    public double MathFloorDouble()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Floor(value);
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 FixedPoint8Floor2()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Floor(2);
        }
        return sum;
    }

    [Benchmark]
    public decimal MathFloor2Decimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Floor(value * 100) / 100;
        }
        return sum;
    }

    [Benchmark]
    public double MathFloor2Double()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Floor(value * 100) / 100;
        }
        return sum;
    }


    // Truncate
    [Benchmark]
    public FixedPoint8 FixedPoint8Truncate()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Truncate();
        }
        return sum;
    }

    [Benchmark]
    public decimal MathTruncateDecimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Truncate(value);
        }
        return sum;
    }

    [Benchmark]
    public double MathTruncateDouble()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Truncate(value);
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 FixedPoint8Truncate2()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Truncate(2);
        }
        return sum;
    }

    [Benchmark]
    public decimal MathTruncate2Decimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Truncate(value * 100) / 100;
        }
        return sum;
    }

    [Benchmark]
    public double MathTruncate2Double()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Truncate(value * 100) / 100;
        }
        return sum;
    }


    // Ceiling
    [Benchmark]
    public FixedPoint8 FixedPoint8Ceiling()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Ceiling();
        }
        return sum;
    }

    [Benchmark]
    public decimal MathCeilingDecimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Ceiling(value);
        }
        return sum;
    }

    [Benchmark]
    public double MathCeilingDouble()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Ceiling(value);
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 FixedPoint8Ceiling2()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var value in _fp8Values)
        {
            sum += value.Ceiling(2);
        }
        return sum;
    }

    [Benchmark]
    public decimal MathCeiling2Decimal()
    {
        decimal sum = 0m;
        foreach (var value in _decimalValues)
        {
            sum += Math.Ceiling(value * 100) / 100;
        }
        return sum;
    }

    [Benchmark]
    public double MathCeiling2Double()
    {
        double sum = 0.0;
        foreach (var value in _doubleValues)
        {
            sum += Math.Ceiling(value * 100) / 100;
        }
        return sum;
    }

//|               Method |            Mean |          Error |         StdDev |          Median |
//|--------------------- |----------------:|---------------:|---------------:|----------------:|
//|              Mul2Int |   4,405.6419 ns |     75.6055 ns |     67.0223 ns |   4,378.1532 ns |
//|           Mul2Double |  11,597.1852 ns |    110.5118 ns |     97.9658 ns |  11,550.6607 ns |
//|          Mul2Decimal | 171,681.9318 ns |  2,222.0380 ns |  1,969.7788 ns | 171,415.0513 ns |
//|   MulInt2FixedPoint8 |   5,095.5557 ns |    229.8811 ns |    652.1343 ns |   5,063.8119 ns |
//|      Mul2FixedPoint8 | 740,576.5320 ns | 14,567.0630 ns | 14,306.8037 ns | 736,481.2988 ns |
//|             Mul10Int |   4,198.6275 ns |     59.8123 ns |     55.9485 ns |   4,207.6736 ns |
//|          Mul10Double |  11,566.6072 ns |     85.9625 ns |     80.4094 ns |  11,562.5351 ns |
//|         Mul10Decimal | 182,478.7274 ns |  3,625.5590 ns |  6,444.4275 ns | 181,066.9189 ns |
//|  MulInt10FixedPoint8 |   7,960.9773 ns |    153.5037 ns |    188.5164 ns |   7,924.3217 ns |
//|     Mul10FixedPoint8 | 737,801.1133 ns | 13,234.3068 ns | 12,379.3788 ns | 739,840.6250 ns |

//|              Add2Int |   5,130.1053 ns |    206.7801 ns |    603.1874 ns |   5,062.9269 ns |
//|           Add2Double |  11,804.3303 ns |    129.1374 ns |    114.4769 ns |  11,841.0278 ns |
//|          Add2Decimal | 229,580.8140 ns |  3,002.1355 ns |  2,661.3149 ns | 229,196.7773 ns |
//|      Add2FixedPoint8 |   8,053.5049 ns |     95.3321 ns |     84.5094 ns |   8,045.7703 ns |
//|             Add10Int |   5,179.5364 ns |    199.3422 ns |    581.4905 ns |   5,021.6770 ns |
//|          Add10Double |  11,711.3880 ns |    170.7424 ns |    159.7126 ns |  11,681.6528 ns |
//|         Add10Decimal | 227,311.7928 ns |  3,070.6181 ns |  2,872.2581 ns | 226,703.4302 ns |
//|     Add10FixedPoint8 |   8,009.1789 ns |     74.9214 ns |     66.4159 ns |   7,994.0407 ns |

//|              Sub2Int |   5,222.6691 ns |    258.3863 ns |    761.8579 ns |   4,992.6422 ns |
//|           Sub2Double |  11,577.0315 ns |     98.2322 ns |     87.0803 ns |  11,558.0177 ns |
//|          Sub2Decimal | 236,974.9669 ns |  3,245.1285 ns |  2,876.7219 ns | 236,256.2500 ns |
//|      Sub2FixedPoint8 |   8,148.7481 ns |    118.4674 ns |     98.9257 ns |   8,159.6352 ns |
//|             Sub10Int |   5,108.3281 ns |    219.5566 ns |    640.4570 ns |   4,884.5806 ns |
//|          Sub10Double |  11,752.1292 ns |    231.5493 ns |    237.7841 ns |  11,619.3405 ns |
//|         Sub10Decimal | 229,858.3089 ns |  2,906.6454 ns |  2,718.8779 ns | 228,971.4355 ns |
//|     Sub10FixedPoint8 |   8,629.7839 ns |    163.8589 ns |    323.4414 ns |   8,561.5463 ns |

//|          LessThanInt |       0.0253 ns |      0.0235 ns |      0.0322 ns |       0.0095 ns |
//|       LessThanDouble |       0.0164 ns |      0.0214 ns |      0.0189 ns |       0.0095 ns |
//|      LessThanDecimal |       1.4452 ns |      0.0210 ns |      0.0164 ns |       1.4495 ns |
//|  LessThanFixedPoint8 |       0.2241 ns |      0.0199 ns |      0.0176 ns |       0.2201 ns |

//|     FixedPoint8Round |      41.7298 ns |      0.8479 ns |      1.5289 ns |      41.6727 ns |
//|     MathRoundDecimal |      37.2170 ns |      0.7726 ns |      0.8587 ns |      37.1739 ns |
//|      MathRoundDouble |       4.8214 ns |      0.1204 ns |      0.2861 ns |       4.7436 ns |
//|    FixedPoint8Round2 |      63.1309 ns |      1.2768 ns |      1.9499 ns |      63.4095 ns |
//|    MathRound2Decimal |      99.7083 ns |      1.9529 ns |      2.0895 ns |      99.7208 ns |
//|     MathRound2Double |      76.5034 ns |      0.5091 ns |      0.4513 ns |      76.3738 ns |

//|     FixedPoint8Floor |      27.6969 ns |      0.5593 ns |      0.7655 ns |      27.3188 ns |
//|     MathFloorDecimal |      91.8603 ns |      1.8089 ns |      1.9355 ns |      91.0285 ns |
//|      MathFloorDouble |       4.7422 ns |      0.1183 ns |      0.2719 ns |       4.6641 ns |
//|    FixedPoint8Floor2 |      39.2432 ns |      0.7993 ns |      1.3998 ns |      39.4201 ns |
//|    MathFloor2Decimal |     388.6078 ns |      7.2770 ns |      7.1470 ns |     391.0600 ns |
//|     MathFloor2Double |      16.5799 ns |      0.3517 ns |      0.4319 ns |      16.4391 ns |

//|  FixedPoint8Truncate |      18.9728 ns |      0.4001 ns |      0.8352 ns |      18.8358 ns |
//|  MathTruncateDecimal |      88.6778 ns |      1.7591 ns |      2.0940 ns |      87.6201 ns |
//|   MathTruncateDouble |       4.8655 ns |      0.1207 ns |      0.3049 ns |       4.7902 ns |
//| FixedPoint8Truncate2 |      26.5847 ns |      0.5454 ns |      0.5600 ns |      26.2340 ns |
//| MathTruncate2Decimal |     393.8051 ns |      6.9726 ns |      6.5221 ns |     393.8026 ns |
//|  MathTruncate2Double |      16.1779 ns |      0.1698 ns |      0.1505 ns |      16.1909 ns |

//|   FixedPoint8Ceiling |      27.2055 ns |      0.5274 ns |      0.8210 ns |      27.0171 ns |
//|   MathCeilingDecimal |      99.3372 ns |      2.0077 ns |      2.5391 ns |      99.7653 ns |
//|    MathCeilingDouble |       4.9575 ns |      0.1227 ns |      0.2940 ns |       4.9235 ns |
//|  FixedPoint8Ceiling2 |      35.8560 ns |      0.7374 ns |      1.4728 ns |      35.8121 ns |
//|  MathCeiling2Decimal |     391.4289 ns |      7.6075 ns |     10.4132 ns |     387.5493 ns |
//|   MathCeiling2Double |      16.0176 ns |      0.0523 ns |      0.0464 ns |      16.0165 ns |
}

