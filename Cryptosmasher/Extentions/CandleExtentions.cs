using Cryptosmasher.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosmasher.Extentions
{
    public static class CandleExtentions
    {
        public static decimal GetMovingAvarage(this List<ICandleStick> candles, int length)
        {
            decimal avg = 0;
            if (length> candles.Count)
            {
                avg = candles.TakeLast(candles.Count).Sum(x => x.Close) / candles.Count;
            }
            else
            {
                avg = candles.TakeLast(length).Sum(x => x.Close) / length;
            }
            
            return avg;
        }

        public static decimal GetCCI(this List<ICandleStick> candles, int length,int length2)
        {
            
            decimal cci = 0;
            if (length < length2 && length > 0)
            {
                cci = Math.Round(candles.TakeLast(length).Average(x=>x.Close)- candles.TakeLast(length2).Average(x => x.Close),8);
            }
            return cci;
        }

        public static void SetLastHighest(this ITradePair ticker)
        {
            var candles_4H = ticker.CandleSticks_4h.OrderByDescending(x => x.Date);

            for (int i = 1; i < candles_4H.Count(); i++)
            {
                if (candles_4H.ElementAt(i).High > candles_4H.First().High)
                {
                    ticker.HighestInXAmountOfCandles_4h = i;
                    break;
                }

                if(i+1== candles_4H.Count())
                {
                    ticker.HighestInXAmountOfCandles_4h = i;
                }
            }

            var candles_Daily = ticker.CandleSticks_Daily.OrderByDescending(x => x.Date);

            for (int i = 1; i < candles_Daily.Count(); i++)
            {
                if (candles_Daily.ElementAt(i).High > candles_Daily.First().High)
                {
                    ticker.HighestInXAmountOfCandles_Daily = i;
                    break;
                }

                if (i + 1 == candles_4H.Count())
                {
                    ticker.HighestInXAmountOfCandles_Daily = i;
                }
            }

            return;
        }

        public static void GetCCICrossToPositive(this ITradePair ticker)
        {
            var candles_4h = ticker.CandleSticks_4h.OrderByDescending(x => x.Date).ToList();            


            if(candles_4h.FirstOrDefault() != null)
            {
                if (candles_4h.FirstOrDefault().CCI < 0)
                {
                    ticker.CCI_positive_distance_4h = 0;
                }
                else
                {
                    for (int i = 1; i < candles_4h.Count(); i++)
                    {
                        if (candles_4h.ElementAt(i).CCI < 0)
                        {
                            ticker.CCI_positive_distance_4h = i;
                            break;
                        }
                    }
                }
            }


            var candles_Daily = ticker.CandleSticks_Daily.OrderByDescending(x => x.Date).ToList();
            if (candles_Daily.FirstOrDefault() != null)
            {
                if (candles_Daily.FirstOrDefault().CCI < 0)
                {
                    ticker.CCI_positive_distance_4h = 0;
                }
                else
                {
                    for (int i = 1; i < candles_Daily.Count(); i++)
                    {
                        if (candles_Daily.ElementAt(i).CCI < 0)
                        {
                            ticker.CCI_positive_distance_Daily = i;
                            break;
                        }
                    }
                }
            }



        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static decimal StdDev(this List<ICandleStick> candles)
        {
            var avarage = candles.Average(x => x.Close);

            var stddev = (decimal)Math.Sqrt(candles.Select(x =>  Math.Pow((double)(avarage - x.Close),2)).Sum()/(double)candles.Count);
            
            return stddev;
        }

    }
}
