namespace Cryptosmasher.Common
{
    public class CandleStick : ICandleStick
    {
        public long Date { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public decimal QuoteVolume { get; set; }
        public decimal WeightedAverage { get; set; }
        public decimal CCI { get; set; }
        public decimal SMA20 { get; set; }

        public CandlestickInterval CandleInterval { get; set; }

        public CandleTrend Trend
        {
            get
            {

                if (Open > Close)
                {
                    return CandleTrend.Red;
                }
                else if (Open < Close)
                {
                    return CandleTrend.Green;
                }
                else
                {
                    return CandleTrend.Neutral;
                }
            }
        }

    }

    public enum CandleTrend
        {
            Green,
            Red,
            Neutral
        }

}
