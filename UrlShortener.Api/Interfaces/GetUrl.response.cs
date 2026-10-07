namespace UrlShortener.Api.Interfaces
{
    public class GetUrlResponse
    {
        public string originalUrl { get; set; } = "";
        public long accessCount { get; set; } = 0;
    }
}
