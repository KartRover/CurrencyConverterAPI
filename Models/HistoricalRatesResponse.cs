namespace CurrencyConverterAPI.Models
{
    public class HistoricalRatesResponse
    {
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
        public string Base { get; set; }
        public string StartAt { get; set; }
        public string EndAt { get; set; }
    }

}
