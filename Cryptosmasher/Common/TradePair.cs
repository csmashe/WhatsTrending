using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosmasher.Common
{
    public class TradePair : ITradePair
    {
        public TradePair()
        {
            CandleSticks_4h = new List<ICandleStick>();
            CandleSticks_Daily = new List<ICandleStick>();
        }
        public string Id { get; set; }
        
        public CryptoExchange Exchange { get; set; }
        public List<ICandleStick> CandleSticks_4h { get; set; }
        public List<ICandleStick> CandleSticks_Daily { get; set; }
        public DateTime LastUpdated { get; set; }

        public int CCI_positive_distance_4h { get; set; }
        public int CCI_positive_distance_Daily { get; set; }

        public int HighestInXAmountOfCandles_4h { get; set; }
        public int HighestInXAmountOfCandles_Daily { get; set; }

        public decimal LastSMA20_4h { get {return CandleSticks_4h.OrderByDescending(x => x.Date).FirstOrDefault().SMA20; } }
        public decimal LastSMA20_Daily { get { return CandleSticks_Daily.OrderByDescending(x => x.Date).FirstOrDefault().SMA20; } }

        public decimal LastClose_4h { get { return CandleSticks_4h.OrderByDescending(x => x.Date).FirstOrDefault().Close; } }
        public decimal LastClose_Daily { get { return CandleSticks_Daily.OrderByDescending(x => x.Date).FirstOrDefault().Close; } }

        private bool CCI_4h_Is_Positive_Distance_Is_1 {
            get {
                var lastCandle = CandleSticks_4h.OrderByDescending(x => x.Date).FirstOrDefault();

                if (lastCandle != null)
                {
                    if (CCI_positive_distance_4h == 1 && lastCandle.SMA20 < lastCandle.Close)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool CCI_Daily_Is_Positive_Distance_Is_1
        {
            get
            {
                var lastCandle = CandleSticks_Daily.OrderByDescending(x => x.Date).FirstOrDefault();

                if (lastCandle != null)
                {
                    if (CCI_positive_distance_Daily == 1 && lastCandle.SMA20 < lastCandle.Close)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool LastCCIPositive_4h {
            get
            {
                var last = CandleSticks_4h.LastOrDefault();
                if (last == null)
                {
                    return false;
                }                

                return CandleSticks_4h.LastOrDefault().CCI > 0;
            }
        }

        public bool LastCCIPositive_Daily
        {
            get
            {
                var last = CandleSticks_Daily.LastOrDefault();
                if (last == null)
                {
                    return false;
                }

                return CandleSticks_Daily.LastOrDefault().CCI > 0;
            }
        }


        public int Color_4h { get {
                int c = 0;
                if (CCI_positive_distance_4h>0)
                {
                    c = 1;
                }
                var lastCandle = CandleSticks_4h.OrderByDescending(x => x.Date).FirstOrDefault();
                
                if(lastCandle != null)
                {
                    if (CCI_positive_distance_4h==1 && lastCandle.SMA20 < lastCandle.Close)
                    {
                        c = 2;
                    }
                }
               
                return c;
            }
        }
        public int Color_Daily
        {
            get
            {
                int c = 0;
                if (CCI_positive_distance_Daily > 0)
                {
                    c = 1;
                }
                var lastCandle = CandleSticks_Daily.OrderByDescending(x => x.Date).FirstOrDefault();

                if (lastCandle != null)
                {
                    if (CCI_positive_distance_Daily == 1 && lastCandle.SMA20 < lastCandle.Close)
                    {
                        c = 2;
                    }
                }

                return c;
            }
        }
        public override string ToString()
        {
            return Id;
        }
    }
}
