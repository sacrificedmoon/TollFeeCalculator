using System;
using System.Linq;

namespace TollFeeCalculator
{
    public class TollFeeCalculator
    {
        public static void Main()
        {
            var inputFile = Environment.CurrentDirectory + "../../../../testData.txt";
            TollFeeCalculator tollFeeCalculator = new TollFeeCalculator();
            tollFeeCalculator.Run(inputFile);
        }

        public void Run(String inputFile)
        {
            DateTime[] tollPasses = ParseTollPasingDataFromFile(inputFile);
            PrintMessage(CalculateTotalFee(tollPasses));
        }

        public void PrintMessage(int totalFeeToPay)
        {
            Console.Write("The total fee for the inputfile is " + totalFeeToPay);
        }

        public DateTime[] ParseTollPasingDataFromFile(String inputFile)
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
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            return dates;
        }

        public int CalculateTotalFee(DateTime[] tollPasses)
        {
            var sortedTollPasses = getOneDate(tollPasses);
            int totalFee = 0;
            foreach (var day in sortedTollPasses)
            {
                DateTime lastedBilledTollPassing = day.First(); //Starting interval
                var IsFirstPassOfDay = true;
                var dailyTotalFee = 0;
                foreach (var passage in day)
                {
                    var diffInMinutes = (passage - lastedBilledTollPassing).TotalMinutes;
                    if (diffInMinutes > 60 || IsFirstPassOfDay)
                    {
                        dailyTotalFee += TollFeePassageCost(passage);
                        IsFirstPassOfDay = false;
                        lastedBilledTollPassing = passage;
                    }
                    else
                    {
                        dailyTotalFee += CalculateHighestCost(lastedBilledTollPassing, passage);
                    }
                }
                totalFee += dailyTotalFee >= 60 ? 60 : dailyTotalFee;
            }
            return totalFee;
        }

        public IGrouping<DateTime, DateTime>[] getOneDate(DateTime[] unsortedTollPassages)
        {
            var sortedTollPassagesDates = unsortedTollPassages.GroupBy(d => d.Date).OrderBy(m => m.Key.TimeOfDay).Take(1).ToArray();
            return sortedTollPassagesDates;
        }

        public int CalculateHighestCost(DateTime first, DateTime second)
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

        public int TollFeePassageCost(DateTime day)
        {
            if (IsDateAFreeDate(day)) return 0;
            int hour = day.Hour;
            int minute = day.Minute;
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

        public bool IsDateAFreeDate(DateTime day)
        {
            return day.Month == 7 || day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
