using System.Collections.Generic;

namespace SC.DevChallenge.Api.Core
{
    public interface IInstrumentPriceProvider
    {
        List<InstrumentPrice> GetInstrumentPrice(InstrumentPriceProp prop);
    }
}
