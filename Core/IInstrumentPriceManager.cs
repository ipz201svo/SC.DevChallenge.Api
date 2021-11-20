using System;
using System.Collections.Generic;

namespace SC.DevChallenge.Api.Core
{
    public interface IInstrumentPriceManager
    {
        decimal GetAverageInstrumentPrice(InstrumentPriceProp priceProp);

        public int DateTimeToTimeSlot(DateTime dateTime);

        public DateTime TimeSlotToDateTime(int timeSlot);

        public DateTime DateTimeToDateTimeFromTimeSlot(DateTime dateTime);

        public decimal GetBenchmark(string portfolio, DateTime date);

        public decimal GetBenchmark(string portfolio, int timeSlot);

        public List<DatePrice> AggregatePrice(string portfolio, DateTime startDate, DateTime endDate, int intervals);
    }
}
