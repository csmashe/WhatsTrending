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


namespace Cryptosmasher.Apis.Binance
{
    public class BinanceApi : IApi
    {
        private readonly IRestClient _restClient;
        
        private readonly Stopwatch _sw = new Stopwatch();

        public BinanceApi()
        {
            _restClient = new RestClient("https://api.binance.com/api/");          
            _sw.Start();
        }

        private void BlockApiCall()
        {
            while (true)
            {
                if (_sw.ElapsedMilliseconds > 100)
                {                   
                    return;
                }                
                Thread.Sleep(10);
            }            
        }

        public IEnumerable<ITradePair> GetTradePairs()
        {

            BlockApiCall();
            const string url = "v3/ticker/bookTicker";
            IEnumerable<ITradePair> response = _restClient.Get<List<BinanceTradePair>>(url);
            _sw.Restart();
            return response.Where(x=>x.Id != "123456");
        }

        public IEnumerable<ICandleStick> GetCandleSticks(ITradePair tp, long start, long end, CandlestickInterval interval)
        {
            BlockApiCall();

            var intervalStr = "";
            switch (interval)
            {
                case CandlestickInterval.Hours_4:
                    intervalStr = "4h";
                    break;
                case CandlestickInterval.Daily:
                    intervalStr = "1d";
                    break;
                case CandlestickInterval.Minutes_5:
                    break;
                case CandlestickInterval.Minutes_15:
                    break;
                case CandlestickInterval.Minutes_30:
                    break;
                case CandlestickInterval.Hours_2:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }

            var url = $"v1/klines?symbol={tp.Id}&interval={intervalStr}";

            var response = _restClient.Get<List<BinanceCandleStick>>(url);
            _sw.Restart();
            return response;
        }

        public ITradePair GetTickersWithCandlesticks(ITradePair tp)
        {
            var candlesticks_4H = GetCandleSticks(tp, 0, 0, CandlestickInterval.Hours_4).ToList();
            var candlesticksDaily = GetCandleSticks(tp, 0, 0, CandlestickInterval.Daily).ToList();

            var count = candlesticks_4H.Count() > candlesticksDaily.Count() ? candlesticks_4H.Count() : candlesticksDaily.Count();

            for (var i = 0; i < count; i++)
            {
                if(i< candlesticks_4H.Count())
                {
                    tp.CandleSticks_4h.Add(candlesticks_4H.ElementAt(i));
                    tp.CandleSticks_4h.Last().CCI = tp.CandleSticks_4h.GetCCI(3, 10);
                    tp.CandleSticks_4h.Last().SMA20 = tp.CandleSticks_4h.GetMovingAvarage(20);
                }

                if (i < candlesticksDaily.Count())
                {
                    tp.CandleSticks_Daily.Add(candlesticksDaily.ElementAt(i));
                    tp.CandleSticks_Daily.Last().CCI = tp.CandleSticks_Daily.Last().CCI = tp.CandleSticks_Daily.GetCCI(3, 10);
                    tp.CandleSticks_Daily.Last().SMA20 = tp.CandleSticks_Daily.GetMovingAvarage(20);
                    
                }
            }

            return tp;
        }

    }

    [JsonConverter(typeof(BinanceTradePairConverter))]
    public class BinanceTradePair : TradePair
    {
  
    }
    
    public class BinanceTradePairConverter : JsonConverter
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
            var tradePair = new BinanceTradePair() { Id = properties.FirstOrDefault()?.Value.ToString() };

            return tradePair;
     
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }

    [JsonConverter(typeof(BinanceCandleStickConverter))]
    public class BinanceCandleStick: CandleStick
    {
        
    }

    public class BinanceCandleStickConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return string.Empty;
            
            if (reader.TokenType == JsonToken.String){ return serializer.Deserialize(reader, objectType); }

            JToken token = JArray.Load(reader);

            return new BinanceCandleStick()
            {
                Date = token[0].Value<long>(),
                Open = token[1].Value<decimal>(),
                High = token[2].Value<decimal>(),
                Low = token[3].Value<decimal>(),
                Close = token[4].Value<decimal>(),
                Volume = token[5].Value<decimal>()
            };

        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }

}
