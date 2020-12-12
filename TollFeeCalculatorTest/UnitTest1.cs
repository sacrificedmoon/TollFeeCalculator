using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TollFeeCalculator;

namespace TollFeeCalculatorTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly string _filePath = Environment.CurrentDirectory + "../../../../testData.txt";

        [TestMethod]
        public void PrintMessageTest()
        {
            string expected = "The total fee for the inputfile is "; 
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                Program.Run(_filePath);
                Assert.IsTrue(sw.ToString().Contains(expected));
            }
        }

        [TestMethod] 
        public void IsFreeTest() 
        {
            //July
            var testDate = DateTime.Parse("2020-07-15 00:00");
            Assert.AreEqual(true, Program.IsFree(testDate));
            //Sunday
            testDate = DateTime.Parse("2020-12-06 15:00");
            Assert.AreEqual(true, Program.IsFree(testDate));
            //Friday
            testDate = DateTime.Parse("2020-10-16 12:30");
            Assert.AreEqual(false, Program.IsFree(testDate));
        }

        [TestMethod]
        public void TollFeePassageCostTest()
        {
            var testDate = DateTime.Parse("2020-12-07 14:15");
            var cost = Program.TollFeePassageCost(testDate);
            Assert.AreEqual(8, cost);
        }

        [TestMethod]
        public void CalculateTotalFeeTest()                                          
        {
            var testDate = new []
            {
                DateTime.Parse("2020-10-22 15:35"), 
                DateTime.Parse("2020-10-22 16:55")
            }; 
            var actual = Program.CalculateTotalFee(testDate);
            Assert.IsTrue(actual <= 60);
        }

        [TestMethod]
        public void CalculateHighestDailyFeeTest()
        {
            var testDate = new[]
            {
                DateTime.Parse("2020-10-22 06:35"),
                DateTime.Parse("2020-10-22 08:40"),
                DateTime.Parse("2020-10-22 11:45"),
                DateTime.Parse("2020-10-22 13:00"),
                DateTime.Parse("2020-10-22 15:35"),
                DateTime.Parse("2020-10-22 16:55"),
                DateTime.Parse("2020-10-22 17:56")                
            };
            var cost = Program.CalculateTotalFee(testDate);
            Assert.IsTrue(cost == 60);
        }

        [TestMethod]
        public void CalculateFeeWithinHour()
        {
            var testDate = new[]
            {
                DateTime.Parse("2020-10-22 15:35"),
                DateTime.Parse("2020-10-22 16:36")
            };
            var expected = 36;
            var actual = Program.CalculateTotalFee(testDate);
            Assert.AreEqual(expected, actual);

            testDate = new[]
            {
                DateTime.Parse("2020-10-22 15:35"),
                DateTime.Parse("2020-10-22 16:35")
            };
            expected = 18;
            actual = Program.CalculateTotalFee(testDate);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculatingHighestCostTest()
        {
            var passages = new DateTime[]
            {
                DateTime.Parse("2020-01-01 14:59"),
                DateTime.Parse("2020-01-01 15:00")
            };
            Assert.IsTrue(Program.CalculateTotalFee(passages) == 13);

            passages = new DateTime[]
            {
                DateTime.Parse("2020-01-01 16:59"),
                DateTime.Parse("2020-01-01 17:00")
            };
            Assert.IsTrue(Program.CalculateTotalFee(passages) == 18);
        }
    }
}
