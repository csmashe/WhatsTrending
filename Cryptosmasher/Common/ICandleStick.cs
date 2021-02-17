namespace Cryptosmasher.Common
{
    public interface ICandleStick
    {
        CandlestickInterval CandleInterval { get; set; }
        decimal CCI { get; set; }
        decimal Close { get; set; }
        long Date { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        decimal Open { get; set; }
        decimal QuoteVolume { get; set; }
        decimal SMA20 { get; set; }
        CandleTrend Trend { get; }
        decimal WeightedAverage { get; set; }
        decimal Volume { get; set; }
    }
}