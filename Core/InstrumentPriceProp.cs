using System;

namespace SC.DevChallenge.Api.Core
{
    public class InstrumentPriceProp
    {
        public string Portfolio { get; set; }

        public string InstrumentOwner { get; set; }

        public string Instrument { get; set; }

        public DateTime Date { get; set; }
    }
}
