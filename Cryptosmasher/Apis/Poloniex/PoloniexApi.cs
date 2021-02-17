
using Cryptosmasher.Common;
using Cryptosmasher.Extentions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Cryptosmasher.Apis.Poloniex
{
    public class PoloniexApi:IApi
    {
        private readonly IRestClient _restClient;

        private readonly Stopwatch _sw = new Stopwatch();
        public PoloniexApi()
        {
            _restClient = new RestClient("https://poloniex.com");
            _sw.Start();
        }

        private void BlockApiCall()
        {
            while (true)
            {
                if (_sw.ElapsedMilliseconds > 200)
                {
                    return;
                }
                Thread.Sleep(10);
            }
        }

        public IEnumerable<ITradePair> GetTradePairs()
        {
            BlockApiCall();
            var response = _restClient.Get<PoloniexTradePairList>("public?command=return24hVolume");
            _sw.Restart();

            return response;
        }

        public IEnumerable<ICandleStick> GetCandleSticks(ITradePair tp, long start, long end,CandlestickInterval interval)
        {
            BlockApiCall();
            var url = "public?command=returnChartData&currencyPair=" + tp.Id + "&start=1405699200&end=9999999999&period=" + (int)interval;
            var response = _restClient.Get<List<PoloniexCandleStick>>(url);
            _sw.Restart();
            return response;
        }

        public ITradePair GetTickersWithCandlesticks(ITradePair poltickers)
        {
             var ticker = new TradePair()
            {
                Id = poltickers.Id,
                Exchange = CryptoExchange.Poloniex,                    
            };
               
            var candlesticks_4H = GetCandleSticks(poltickers, 0, 0, CandlestickInterval.Hours_4).ToList();
            var candlesticksDaily = GetCandleSticks(poltickers, 0, 0, CandlestickInterval.Daily).ToList();
            
            var count = candlesticks_4H.Count() > candlesticksDaily.Count() ? candlesticks_4H.Count() : candlesticksDaily.Count();
            for (var j = 0; j < count; j++)
            {

                if (j < candlesticks_4H.Count())
                {
                    ticker.CandleSticks_4h.Add(new CandleStick()
                    {
                        CandleInterval= CandlestickInterval.Hours_4,
                        Close = candlesticks_4H.ElementAt(j).Close,
                        High = candlesticks_4H.ElementAt(j).High,
                        Low = candlesticks_4H.ElementAt(j).Low,
                        Open = candlesticks_4H.ElementAt(j).Open,
                        Volume = candlesticks_4H.ElementAt(j).Volume,
                        Date = candlesticks_4H.ElementAt(j).Date  
                    });
                    ticker.CandleSticks_4h.Last().CCI = ticker.CandleSticks_4h.GetCCI(3, 10);
                    ticker.CandleSticks_4h.Last().SMA20 = ticker.CandleSticks_4h.GetMovingAvarage(20);
                }

                if(j< candlesticksDaily.Count())
                {
                    ticker.CandleSticks_Daily.Add(new CandleStick()
                    {
                        CandleInterval = CandlestickInterval.Daily,
                        Close = candlesticksDaily.ElementAt(j).Close,
                        High = candlesticksDaily.ElementAt(j).High,
                        Low = candlesticksDaily.ElementAt(j).Low,
                        Open = candlesticksDaily.ElementAt(j).Open,
                        Volume = candlesticksDaily.ElementAt(j).Volume,
                        Date = candlesticksDaily.ElementAt(j).Date
                    });
                    ticker.CandleSticks_Daily.Last().CCI = ticker.CandleSticks_Daily.GetCCI(3, 10);
                    ticker.CandleSticks_Daily.Last().SMA20 = ticker.CandleSticks_Daily.GetMovingAvarage(20);

                }
            }
            ticker.GetCCICrossToPositive();
            ticker.SetLastHighest();
            ticker.LastUpdated = DateTime.Now;
            
            return ticker;
        }
    }

    [JsonConverter(typeof(PoloniexTradePairListConverter))]
    public class PoloniexTradePairList : List<PoloniexTradePair>
    {
    }

    public class PoloniexTradePair:TradePair
    {       
    }

    [JsonConverter(typeof(PoloniexCandleStickConverter))]
    public class PoloniexCandleStick:CandleStick
    {    

    }

    #region converters

    public class PoloniexTradePairListConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return string.Empty;
            
            if (reader.TokenType == JsonToken.String) return serializer.Deserialize(reader, objectType);
            
            var obj = JObject.Load(reader);
            var properties = obj.Properties().ToList();
            var tradePairs = new PoloniexTradePairList();
            tradePairs.AddRange(properties.Select(x => new PoloniexTradePair() {  Id = x.Name }).Where(y => !y.Id.Contains("total")).ToList());
            return tradePairs;
            
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }

    public class PoloniexCandleStickConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return string.Empty;
            
            if (reader.TokenType == JsonToken.String) return serializer.Deserialize(reader, objectType);
            
            var token = JToken.Load(reader);

            return new PoloniexCandleStick()
            {
                Date = token["date"].Value<long>(),
                Open = token["open"].Value<decimal>(),
                High = token["high"].Value<decimal>(),
                Low = token["low"].Value<decimal>(),
                Close = token["close"].Value<decimal>(),
                Volume = token["volume"].Value<decimal>()
            };
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
#endregion
}
