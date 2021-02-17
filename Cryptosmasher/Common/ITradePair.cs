using System;
using System.Collections.Generic;

namespace Cryptosmasher.Common
{
    public interface ITradePair
    {
        List<ICandleStick> CandleSticks_4h { get; set; }
        List<ICandleStick> CandleSticks_Daily { get; set; }
        int CCI_positive_distance_4h { get; set; }
        int CCI_positive_distance_Daily { get; set; }
        int Color_4h { get; }
        int Color_Daily { get; }
        CryptoExchange Exchange { get; set; }
        int HighestInXAmountOfCandles_4h { get; set; }
        int HighestInXAmountOfCandles_Daily { get; set; }
        string Id { get; set; }
        bool LastCCIPositive_4h { get; }
        bool LastCCIPositive_Daily { get; }
        decimal LastClose_4h { get; }
        decimal LastClose_Daily { get; }
        decimal LastSMA20_4h { get; }
        decimal LastSMA20_Daily { get; }
        DateTime LastUpdated { get; set; }

        string ToString();
    }
}