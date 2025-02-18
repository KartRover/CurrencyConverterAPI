using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public CurrencyController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [Authorize(Roles = "User")]
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            var rates = await _exchangeRateService.GetLatestRatesAsync(baseCurrency);
            return Ok(rates);
        }

        [Authorize(Roles = "User")]
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertCurrency([FromBody] ConversionRequest request)
        {
            if (new[] { "TRY", "PLN", "THB", "MXN" }.Contains(request.FromCurrency) || new[] { "TRY", "PLN", "THB", "MXN" }.Contains(request.ToCurrency))
            {
                return BadRequest("Currency not supported.");
            }

            var result = await _exchangeRateService.ConvertCurrencyAsync(request.FromCurrency, request.ToCurrency, request.Amount);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var rates = await _exchangeRateService.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, pageNumber, pageSize);
            return Ok(rates);
        }
    }

}
