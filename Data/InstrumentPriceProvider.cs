using SC.DevChallenge.Api.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SC.DevChallenge.Api.Data
{
    public class InstrumentPriceProvider : IInstrumentPriceProvider
    {
        protected List<InstrumentPrice> data;

        public InstrumentPriceProvider()
        {
            using var reader = new StreamReader(@"Input\data.csv");
            data = new List<InstrumentPrice>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                var instrumentPrice = new InstrumentPrice
                {
                    Portfolio = values[0],
                    InstrumentOwner = values[1],
                    Instrument = values[2],
                    Date = DateTime.Parse(values[3]),
                    Price = Convert.ToDecimal(values[4], CultureInfo.InvariantCulture)
                };

                data.Add(instrumentPrice);
            }
        }

        public List<InstrumentPrice> GetInstrumentPrice(InstrumentPriceProp prop)
        {
            var instrumentPriceList = data.FindAll(i =>
            (prop.Portfolio == null || i.Portfolio.ToLower() == prop.Portfolio.ToLower())
            && (prop.InstrumentOwner == null || i.InstrumentOwner.ToLower() == prop.InstrumentOwner.ToLower())
            && (prop.Instrument == null || i.Instrument.ToLower() == prop.Instrument.ToLower()));
            return instrumentPriceList;
        }

        public List<InstrumentPrice> GetInstrumentPriceSorted(InstrumentPriceProp prop)
        {
            var instrumentPriceList = GetInstrumentPrice(prop);
            return instrumentPriceList.OrderBy(i => i.Price).ToList();
        }
    }
}
