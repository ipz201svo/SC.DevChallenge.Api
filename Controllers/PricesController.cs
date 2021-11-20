using Microsoft.AspNetCore.Mvc;
using SC.DevChallenge.Api.Core;
using SC.DevChallenge.Api.Infrastructure;
using SC.DevChallenge.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SC.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        private IInstrumentPriceManager instrumentPriceManager;

        public PricesController(IInstrumentPriceManager instrumentPriceManager)
        {
            this.instrumentPriceManager = instrumentPriceManager;
        }

        [HttpGet("average")]
        public ActionResult<InstrumentPriceResponse> Average(string portfolio, string owner, string instrument, DateTime? date)
        {
            if (portfolio == null
                && owner == null
                && instrument == null
                || date == null)
            {
                return NotFound();
            }

            var priceProp = new InstrumentPriceProp
            {
                Portfolio = portfolio,
                InstrumentOwner = owner,
                Instrument = instrument,
                Date = date.Value
            };

            try
            {
                var averagePrice = instrumentPriceManager.GetAverageInstrumentPrice(priceProp);
                var response = new InstrumentPriceResponse
                {
                    Price = string.Format(new CultureInfo("en-US"), "{0:N2}", averagePrice),
                    Date = instrumentPriceManager.DateTimeToDateTimeFromTimeSlot(date.Value).ToString(CultureInfo.CreateSpecificCulture("fr-FR"))
                };

                return response;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("benchmark")]
        public ActionResult<InstrumentPriceResponse> Benchmark(string portfolio, DateTime? date)
        {
            if (portfolio == null || date == null)
            {
                return NotFound();
            }

            var priceProp = new InstrumentPriceProp
            {
                Portfolio = portfolio,
                Date = date.Value
            };
            var averagePrice = instrumentPriceManager.GetAverageInstrumentPrice(priceProp);

            var response = new InstrumentPriceResponse
            {
                Price = string.Format(new CultureInfo("en-US"), "{0:N2}", averagePrice),
                Date = instrumentPriceManager.DateTimeToDateTimeFromTimeSlot(date.Value).ToString(CultureInfo.CreateSpecificCulture("fr-FR"))
            };
            return response;
        }

        [HttpGet("aggregate")]
        public ActionResult<List<InstrumentPriceResponse>> Aggregate(string portfolio, DateTime? startDate, DateTime? endDate, int? intervals)
        {
            if (portfolio == null
                || startDate == null
                || endDate == null
                || intervals == null)
            {
                return NotFound();
            }

            try
            {
                var datePriceList = instrumentPriceManager.AggregatePrice(portfolio, startDate.Value, endDate.Value, intervals.Value);
                var response = datePriceList.Select(dp => new InstrumentPriceResponse() 
                { 
                    Date = dp.Date.ToString(CultureInfo.CreateSpecificCulture("fr-FR")),
                    Price = string.Format(new CultureInfo("en-US"), "{0:N2}", dp.Price )
                }).ToList();
                return response;
            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex);
            }

        }
    }
}
