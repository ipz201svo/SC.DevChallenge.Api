using System;

namespace SC.DevChallenge.Api.Core
{
    public interface IInstrumentPriceManager
    {
        decimal GetAverageInstrumentPrice(InstrumentPriceProp priceProp);

        public int DateTimeToTimeSlot(DateTime dateTime);

        public DateTime TimeSlotToDateTime(int timeSlot, DateTime dateTime);

        public DateTime DateTimeToDateTimeFromTimeSlot(DateTime dateTime);
    }
}
