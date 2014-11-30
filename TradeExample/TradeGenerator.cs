using System;
using System.Collections.Generic;
using System.Linq;

namespace TradeExample
{
    public class TradeGenerator
    {
        private readonly Random _random= new Random();
        private readonly IStaticData _staticData;
     
        public TradeGenerator(IStaticData staticData)
        {
            _staticData = staticData;
        }

        public IEnumerable<Trade> Generate(int numberToGenerate, bool initialLoad=false)
        {


            Func<Trade> newTrade = () =>
                                   {
                                       var id = Guid.NewGuid().GetHashCode();
                                       var bank = _staticData.Customers[_random.Next(0, _staticData.Customers.Length)];
                                       var pair = _staticData.CurrencyPairs[_random.Next(0, _staticData.CurrencyPairs.Length )];

                                       if (initialLoad)
                                       {

                                           var status = _random.NextDouble() > 0.5 ? TradeStatus.Live : TradeStatus.Closed;
                                           var seconds = _random.Next(1, 60*60*24);
                                           var time = DateTime.Now.AddSeconds(-seconds);
                                           return new Trade(id, bank, pair.Code, status, GererateRandomPrice(pair.InitialPrice),timeStamp: time);
                                       }
                                       return new Trade(id, bank, pair.Code, TradeStatus.Live, GererateRandomPrice(pair.InitialPrice));
                                   };
            return Enumerable.Range(1, numberToGenerate).Select(_ => newTrade());
        }


        private decimal GererateRandomPrice(decimal initalPrice)
        {
            //generate percent price 1-10% away from the market
            var pcFromMarket = _random.Next(1, 1000)/(decimal) 10000;

            var positive = _random.NextDouble() > 0.5;

            return positive
                ? initalPrice + (pcFromMarket*initalPrice)
                : initalPrice - (pcFromMarket*initalPrice);
        }
    }
}