using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Api.Models
{

    [Table("t_urls")]
    public class Url
    {
        public long id { get; set; }

        public string code { get; set; } = string.Empty;

        public string original_url { get; set; } = string.Empty;

        public DateTime created_at { get; set; }

        public DateTime? expires_at { get; set; }

        public long click_count { get; set; }
    }
}
