using SC.DevChallenge.Api.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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

                var instrumentPrice = new InstrumentPrice();
                instrumentPrice.Portfolio = values[0];
                instrumentPrice.InstrumentOwner = values[1];
                instrumentPrice.Instrument = values[2];
                instrumentPrice.Date = DateTime.Parse(values[3]);
                instrumentPrice.Price = Convert.ToDecimal(values[4], CultureInfo.InvariantCulture);

                data.Add(instrumentPrice);
            }
        }

        public List<InstrumentPrice> GetInstrumentPrice(InstrumentPriceProp prop)
        {
            var instrumentPriceList = data.FindAll((i) =>
            i.Portfolio.ToLower() == prop.Portfolio.ToLower()
            && i.InstrumentOwner.ToLower() == prop.InstrumentOwner.ToLower()
            && i.Instrument.ToLower() == prop.Instrument.ToLower());
            return instrumentPriceList;
        }
    }
}
