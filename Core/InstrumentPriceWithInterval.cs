using System.Collections.Generic;

namespace SC.DevChallenge.Api.Core
{
    public class InstrumentPriceWithInterval
    {
        public string Portfolio { get; set; }

        public DatePrice DatePrice { get; set; }

        public List<int> TimeSlots { get; set; }
    }
}
