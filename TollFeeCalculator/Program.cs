using System;
using System.Linq;

namespace TollFeeCalculator
{
    public class Program
    {
        public static void Main()
        {
            var inputFile = Environment.CurrentDirectory + "../../../../testData.txt";
            Run(inputFile);
        }

        public static void Run(String inputFile)
        {
            DateTime[] tollPasses = ParseDataFromFile(inputFile);
            PrintMessage(CalculateTotalFee(tollPasses));
        }

        public static void PrintMessage(int totalFeeToPay)
        {
            Console.Write("The total fee for the inputfile is " + totalFeeToPay);
        }

        public static DateTime[] ParseDataFromFile(String inputFile)
        {
            string indata;
            indata = System.IO.File.ReadAllText(inputFile);
            String[] dateStrings = indata.Split(",");
            DateTime[] dates = new DateTime[dateStrings.Length];
            for (int i = 0; i < dates.Length; i++)
            {
                try
                {
                    dates[i] = DateTime.Parse(dateStrings[i]);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return dates;
        }

        public static int CalculateTotalFee(DateTime[] tollPasses)
        {
            var sortedPasses = getOneDate(tollPasses);
            int totalFee = 0;
            foreach (var day in sortedPasses)
            {
                DateTime lastedBilled = day.First(); //Starting interval
                var IsFirstPassOfDay = true;
                var dailyFee = 0;
                foreach (var pass in day)
                {
                    var diffInMinutes = (pass - lastedBilled).TotalMinutes;
                    if (diffInMinutes > 60 || IsFirstPassOfDay)
                    {
                        dailyFee += TollFeePassageCost(pass);
                        IsFirstPassOfDay = false;
                        lastedBilled = pass;
                    }
                    else
                    {
                        dailyFee += CalculateHighestCost(lastedBilled, pass);
                    }
                }
                totalFee += dailyFee >= 60 ? 60 : dailyFee;
            }
            return totalFee;
        }

        public static IGrouping<DateTime, DateTime>[] getOneDate(DateTime[] unsortedPasses)
        {
            var sortedDates = unsortedPasses.GroupBy(d => d.Date).OrderBy(m => m.Key.TimeOfDay).Take(1).ToArray();
            return sortedDates;
        }

        public static int CalculateHighestCost(DateTime first, DateTime second)
        {
            int highestFee;
            if (TollFeePassageCost(first) >= TollFeePassageCost(second))
            {
                highestFee = 0;
            }
            else
            {
                highestFee = TollFeePassageCost(second) - TollFeePassageCost(first);
            }
            return highestFee;
        }

        public static int TollFeePassageCost(DateTime d)
        {
            if (IsFree(d)) return 0;
            int hour = d.Hour;
            int minute = d.Minute;
            if (hour == 6 && minute >= 0 && minute <= 29) return 8;
            else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
            else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
            else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
            else if (hour == 8 && minute >= 30 && minute <= 59) return 8;
            else if (hour >= 9 && hour <= 14 && minute >= 0 && minute <= 59) return 8;
            else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
            else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
            else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
            else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
            else return 0;
        }

        static public bool IsFree(DateTime day)
        {
            return day.Month == 7 || day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
