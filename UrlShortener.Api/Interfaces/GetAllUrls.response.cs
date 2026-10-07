namespace UrlShortener.Api.Interfaces
{
    public class GetAllUrlsResponse
    {
        public Object url_data { get; set; } = Array.Empty<UrlDataResponse>();
    }

    public class UrlDataResponse
    {
        public string code { get; set; } = string.Empty;
        public string original_url { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public DateTime? expires_at { get; set; }
        public long click_count { get; set; }
    }
}
