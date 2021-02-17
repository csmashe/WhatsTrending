using System.Collections.Generic;
using Cryptosmasher.Common;

namespace Cryptosmasher.Common
{
    public interface IApi
    {
        IEnumerable<ICandleStick> GetCandleSticks(ITradePair tp, long start, long end, CandlestickInterval interval);
        ITradePair GetTickersWithCandlesticks(ITradePair tp);
        IEnumerable<ITradePair> GetTradePairs();
    }
}