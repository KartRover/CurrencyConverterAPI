using CurrencyConverterAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Registry;

namespace CurrencyConverterAPI.Services
{
    public class ExchangeRateService: IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        public ExchangeRateService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            var rates = await GetLatestRatesAsync(fromCurrency);
            if (rates.Rates.TryGetValue(toCurrency, out var rate))
            {
                return amount * rate;
            }
            throw new ArgumentException("Invalid currency code.");
        }

        public async Task<IEnumerable<ExchangeRate>> GetHistoricalRatesAsync(string baseCurrency, DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var response = await _httpClient.GetAsync($"https://api.frankfurter.dev/v1/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}");
            response.EnsureSuccessStatusCode();
            var ratesResponse = await response.Content.ReadFromJsonAsync<HistoricalRatesResponse>();
            
            return ratesResponse.Rates.Select(rate => new ExchangeRate
            {
                BaseCurrency = baseCurrency,
                Date = DateTime.Parse(rate.Key),
                Rates = rate.Value
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public async Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency)
        {
            if (_cache.TryGetValue(baseCurrency, out ExchangeRate cachedRates))
            {
                return cachedRates;
            }

            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?base={baseCurrency}");
            response.EnsureSuccessStatusCode();
            var rates = await response.Content.ReadFromJsonAsync<ExchangeRate>();

            _cache.Set(baseCurrency, rates, TimeSpan.FromMinutes(10));
            return rates;
        }
    }
}
