using Microsoft.AspNetCore.Mvc;
using SC.DevChallenge.Api.Core;
using SC.DevChallenge.Api.Models;
using System;
using System.Globalization;

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
        public ActionResult<InstrumentAveragePriceResponse> Average(string portfolio, string owner, string instrument, DateTime? datetime)
        {
            if (portfolio == null || owner == null || instrument == null || datetime == null)
            {
                return NotFound();
            }

            var priceProp = new InstrumentPriceProp();
            priceProp.Portfolio = portfolio;
            priceProp.InstrumentOwner = owner;
            priceProp.Instrument = instrument;
            priceProp.Date = datetime.Value;

            try
            {
                var averagePrice = instrumentPriceManager.GetAverageInstrumentPrice(priceProp);
                var response = new InstrumentAveragePriceResponse();
                response.Price = string.Format("{0:N2}", averagePrice);
                response.Date = instrumentPriceManager.DateTimeToDateTimeFromTimeSlot(datetime.Value).ToString(CultureInfo.InvariantCulture);

                return response;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
