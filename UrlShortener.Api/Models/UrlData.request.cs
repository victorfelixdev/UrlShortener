namespace UrlShortener.Api.Models
{
    public class UrlData
    {
        public string url { get; set; } = string.Empty;
        public string? customCode { get; set; }
    }
}
