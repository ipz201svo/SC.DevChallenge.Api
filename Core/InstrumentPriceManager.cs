using System;
using System.Collections.Generic;
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
            int timeSlot = (int)(seconds / 10000);
            return timeSlot;
        }

        public DateTime TimeSlotToDateTime(int timeSlot)
        {
            var timeSpanFromSeconds = TimeSpan.FromSeconds(timeSlot * 10000);
            var timeSlotDateTime = new DateTime(2018, 1, 1) + timeSpanFromSeconds;
            return timeSlotDateTime;
        }

        public DateTime DateTimeToDateTimeFromTimeSlot(DateTime dateTime)
        {
            var timeSlot = DateTimeToTimeSlot(dateTime);
            var dateTimeFromTimeSlot = TimeSlotToDateTime(timeSlot);
            return dateTimeFromTimeSlot;
        }

        public decimal GetBenchmark(string portfolio, DateTime date)
        {
            var instrumentPriceProp = new InstrumentPriceProp() { Date = date, Portfolio = portfolio };
            var timeSlot = DateTimeToTimeSlot(date);
            // Instrument prices for portfolio in the current time slot
            var instrumentPrices = instrumentPriceProvider
                .GetInstrumentPriceSorted(instrumentPriceProp)
                .Where(i => DateTimeToTimeSlot(i.Date) == timeSlot)
                .ToList();

            if (instrumentPrices.Count == 0)
                return 0;


            decimal lowerFence;
            decimal upperFence;
            if (instrumentPrices.Count == 1)
            {
                lowerFence = instrumentPrices[0].Price;
                upperFence = instrumentPrices[0].Price;
            }
            else if (instrumentPrices.Count == 2)
            {
                lowerFence = instrumentPrices[0].Price;
                upperFence = instrumentPrices[1].Price;
            }
            else if (instrumentPrices.Count == 3)
            {
                lowerFence = instrumentPrices[0].Price;
                upperFence = instrumentPrices[2].Price;
            }
            else
            {
                var fences = GetQuartileFences(instrumentPrices);
                lowerFence = fences[0];
                upperFence = fences[1];
            }


            // Insrtument prices that are in the proper range
            var instrumentPricesInRange = instrumentPrices.Where(i => i.Price >= lowerFence && i.Price <= upperFence);

            var average = instrumentPricesInRange.Select(i => i.Price).Average();
            return average;
        }

        public decimal GetBenchmark(string portfolio, int timeSlot)
        {
            var time = TimeSlotToDateTime(timeSlot);
            return GetBenchmark(portfolio, time);
        }

        public List<DatePrice> AggregatePrice(string portfolio, DateTime startDate, DateTime endDate, int intervals)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("End date should be later than start.");
            }

            var startTimeSlot = DateTimeToTimeSlot(startDate);
            var endTimeSlot = DateTimeToTimeSlot(endDate);
            var timeSlotCount = endTimeSlot - startTimeSlot;
            var intervalDuration = (int)Math.Floor((double)(timeSlotCount / intervals));

            if (intervalDuration < 1)
            {
                intervalDuration = 1;
            }

            var instrumentPriceWithIntervalList = new List<InstrumentPriceWithInterval>();
            // calculating a duariton and the number of the intervals due to leftover principle
            var remainder = timeSlotCount - intervalDuration * intervals;
            for (int i = 0; i < intervals; i++)
            {
                var currentIntervalDuration = intervalDuration;
                if (remainder > 0)
                    currentIntervalDuration++;
                remainder--;
                var timeSlots = new List<int>();
                for (int j = 0; j < currentIntervalDuration; j++)
                {
                    timeSlots.Add(startTimeSlot + i * currentIntervalDuration + j);
                }
                instrumentPriceWithIntervalList.Add(new InstrumentPriceWithInterval()
                {
                    Portfolio = portfolio,
                    TimeSlots = timeSlots,
                    DatePrice = new DatePrice()
                });
            }

            // calculating benchmark for every group of time slots
            foreach (var instrumentPriceWithInterval in instrumentPriceWithIntervalList)
            {
                var datePriceForTimeSlotList = new List<DatePrice>();
                foreach (var timeSlot in instrumentPriceWithInterval.TimeSlots)
                {
                    var datePrice = new DatePrice
                    {
                        Price = GetBenchmark(instrumentPriceWithInterval.Portfolio, timeSlot),
                        Date = TimeSlotToDateTime(timeSlot)
                    };

                    datePriceForTimeSlotList.Add(datePrice);
                }
                instrumentPriceWithInterval.DatePrice.Price = datePriceForTimeSlotList
                    .Select(dp => dp.Price)
                    .Where(p => p != 0)
                    .Average();
                instrumentPriceWithInterval.DatePrice.Date = datePriceForTimeSlotList
                    .Select(dp => dp.Date)
                    .First();
            }

            return instrumentPriceWithIntervalList.Select(ip => ip.DatePrice).ToList();
        }

        protected decimal[] GetQuartileFences(List<InstrumentPrice> instrumentPrices)
        {
            var count = instrumentPrices.Count;
            var q1 = (int)Math.Ceiling((count - 1) / 4d);
            var q3 = (int)Math.Ceiling((3 * count - 3) / 4d);

            var iqr = instrumentPrices[q3].Price - instrumentPrices[q1].Price;
            var lowerFence = instrumentPrices[q1].Price - 1.5m * iqr;
            var upperFence = instrumentPrices[q3].Price + 1.5m * iqr;
            return new decimal[] { lowerFence, upperFence };
        }
    }
}
