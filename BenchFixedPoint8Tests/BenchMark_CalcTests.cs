using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gitan.FixedPoint8.Tests
{
    [TestClass()]
    public class BenchMark_CalcTests
    {
        static readonly BenchMark_Calc instance = new();

        /////////////////////////////////////// multiplication	

        [TestMethod()]
        public void Mul2Tests()
        {
            var resultInt = instance.Mul2Int();
            Assert.IsTrue(resultInt.Equals(0));

            var resultDecimal = instance.Mul2Decimal();
            Assert.IsTrue(resultDecimal.Equals(-0m));

            var resultDouble = instance.Mul2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.Mul2FixedPoint8();
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }     
        
        [TestMethod()]
        public void Mul10Tests()
        {
            var resultInt = instance.Mul10Int();
            Assert.IsTrue(resultInt.Equals(-209780000));

            var resultDecimal = instance.Mul10Decimal();
            Assert.IsTrue(resultDecimal.Equals(0m));

            var resultDouble = instance.Mul10Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.Mul10FixedPoint8();
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }



        /////////////////////////////////////// addition

        [TestMethod()]
        public void Add2Tests()
        {
            var resultInt = instance.Add2Int();
            Assert.IsTrue(resultInt.Equals(34000));

            var resultDecimal = instance.Add2Decimal();
            Assert.IsTrue(resultDecimal.Equals(34000m));

            var resultDouble = instance.Add2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.Add2FixedPoint8();
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }

        [TestMethod()]
        public void Add10Tests()
        {
            var resultInt = instance.Add10Int();
            Assert.IsTrue(resultInt.Equals(170000));

            var resultDecimal = instance.Add10Decimal();
            Assert.IsTrue(resultDecimal.Equals(170000m));

            var resultDouble = instance.Add10Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.Add10FixedPoint8();
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        /////////////////////////////////////// subtraction	

        [TestMethod()]
        public void Sub2Tests()
        {
            var resultInt = instance.Sub2Int();
            Assert.IsTrue(resultInt.Equals(-34000));

            var resultDecimal = instance.Sub2Decimal();
            Assert.IsTrue(resultDecimal.Equals(-34000m));

            var resultDouble = instance.Sub2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.Sub2FixedPoint8();
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }

        [TestMethod()]
        public void Sub10Tests()
        {
            var resultInt = instance.Sub10Int();
            Assert.IsTrue(resultInt.Equals(-170000));

            var resultDecimal = instance.Sub10Decimal();
            Assert.IsTrue(resultDecimal.Equals(-170000m));

            var resultDouble = instance.Sub10Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.Sub10FixedPoint8();
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        /////////////////////////////////////// Less than

        [TestMethod()]
        public void LessThanTests()
        {
            var resultInt = instance.LessThanInt();
            Assert.IsTrue(resultInt);

            var resultDouble = instance.LessThanDouble();
            Assert.IsTrue(resultDouble);

            var resultDecimal = instance.LessThanDecimal();
            Assert.IsTrue(resultDecimal);

            var resultFp8 = instance.LessThanFixedPoint8();
            Assert.IsTrue(resultFp8);

            Assert.IsTrue(resultDecimal == resultFp8);
        }


        /////////////////////////////////////// Math

        // Round
        [TestMethod()]
        public void RoundTests()
        {
            var resultDecimal = instance.MathRoundDecimal();
            Assert.IsTrue(resultDecimal == 0m);

            var resultDouble = instance.MathRoundDouble();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Round();
            Assert.IsTrue(resultFp8 == FixedPoint8.Zero);

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        [TestMethod()]
        public void Round2Test()
        {
            var resultDecimal = instance.MathRound2Decimal();
            Assert.IsTrue(resultDecimal == 0m);

            var resultDouble = instance.MathRound2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Round2();
            Assert.IsTrue(resultFp8 == FixedPoint8.Zero);

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        // Floor
        [TestMethod()]
        public void FloorTest()
        {
            var resultDecimal = instance.MathFloorDecimal();
            Assert.IsTrue(resultDecimal == -4m);

            var resultDouble = instance.MathFloorDouble();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Floor();
            Assert.IsTrue(resultFp8 == (FixedPoint8)(-4m));

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        [TestMethod()]
        public void Floor2Test()
        {
            var resultDecimal = instance.MathFloor2Decimal();
            Assert.IsTrue(resultDecimal == -0.02m);

            var resultDouble = instance.MathFloor2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Floor2();
            Assert.IsTrue(resultFp8 == (FixedPoint8)(-0.02m));

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        // Truncate
        [TestMethod()]
        public void TruncateTest()
        {
            var resultDecimal = instance.MathTruncateDecimal();
            Assert.IsTrue(resultDecimal == 0m);

            var resultDouble = instance.MathTruncateDouble();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Truncate();
            Assert.IsTrue(resultFp8 == FixedPoint8.Zero);

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        [TestMethod()]
        public void Truncate2Test()
        {
            var resultDecimal = instance.MathTruncate2Decimal();
            Assert.IsTrue(resultDecimal == 0m);

            var resultDouble = instance.MathTruncate2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Truncate2();
            Assert.IsTrue(resultFp8 == FixedPoint8.Zero);

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        // Ceiling
        [TestMethod()]
        public void CeilingTest()
        {
            var resultDecimal = instance.MathCeilingDecimal();
            Assert.IsTrue(resultDecimal == 4m);

            var resultDouble = instance.MathCeilingDouble();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Ceiling();
            Assert.IsTrue(resultFp8 == (FixedPoint8)(4m));

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        [TestMethod()]
        public void Ceiling2Test()
        {
            var resultDecimal = instance.MathCeiling2Decimal();
            Assert.IsTrue(resultDecimal == 0.02m);

            var resultDouble = instance.MathCeiling2Double();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.FixedPoint8Ceiling2();
            Assert.IsTrue(resultFp8 == (FixedPoint8)(0.02m));

            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }
    }
}