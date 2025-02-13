using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency);
        Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount);
        Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    }}
