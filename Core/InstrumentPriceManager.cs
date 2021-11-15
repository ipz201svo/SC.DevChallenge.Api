using System;
using System.Linq;

namespace SC.DevChallenge.Api.Core
{
    public class InstrumentPriceManager : IInstrumentPriceManager
    {
        private IInstrumentPriceProvider instrumentPriceProvider;

        public InstrumentPriceManager(IInstrumentPriceProvider instrumentPriceProvider)
        {
            this.instrumentPriceProvider = instrumentPriceProvider;
        }

        public decimal GetAverageInstrumentPrice(InstrumentPriceProp priceProp)
        {
            var instruments = instrumentPriceProvider.GetInstrumentPrice(priceProp);
            if (instruments == null)
                throw new Exception("Instrument not found");
            var timeSlot = DateTimeToTimeSlot(priceProp.Date);
            var instrumentsInTimeSlot = instruments.Where(i => DateTimeToTimeSlot(i.Date) == timeSlot).ToList();
            var averagePrice = instrumentsInTimeSlot.Select(i => i.Price).Average();
            return averagePrice;
        }

        public int DateTimeToTimeSlot(DateTime dateTime)
        {
            var seconds = (dateTime - new DateTime(2018, 1, 1)).TotalSeconds;
            var timeSlot = (int)Math.Truncate(seconds / 10000);
            return timeSlot;
        }

        public DateTime TimeSlotToDateTime(int timeSlot, DateTime dateTime)
        {
            var timeSpanFromSeconds = TimeSpan.FromSeconds(timeSlot * 10000);
            var timeSlotDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day) + timeSpanFromSeconds;
            return timeSlotDateTime;
        }

        public DateTime DateTimeToDateTimeFromTimeSlot(DateTime dateTime)
        {
            var timeSlot = DateTimeToTimeSlot(dateTime);
            var dateTimeFromTimeSlot = TimeSlotToDateTime(timeSlot, dateTime);
            return dateTimeFromTimeSlot;
        }
    }
}
