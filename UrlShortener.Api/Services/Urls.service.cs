using System.Security.Cryptography;
using System.Text;
using UrlShortener.Api.Models;
using UrlShortener.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Runtime.CompilerServices;
using UrlShortener.Api.Interfaces;

namespace UrlShortener.Api.Services
{
    public class Urls
    {
        private readonly AppDbContext _db;

        public Urls(AppDbContext db)
        {
            _db = db;
        }
        private bool IsValideCustomCode(string custom_code)
        {
            if (custom_code == null || custom_code == string.Empty) return false;
            var db_check_if_custom_code_exists = _db.Urls.FirstOrDefaultAsync(url => url.code == custom_code);
            return db_check_if_custom_code_exists.Result == null ? true : false;
        }

        private static string GenerateNewCode()
        {
            string now = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string randomSalt = Random.Shared.Next(1000000, 9999999).ToString();
            string hashSalt = now + randomSalt;
            byte[] inputBytes = Encoding.UTF8.GetBytes(hashSalt);
            byte[] hashBytes = MD5.HashData(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private string GenerateCustomCode(string? custom_code = "")
        {
            var new_custom_code = string.Empty;

            if ((string.IsNullOrWhiteSpace(custom_code) || custom_code == string.Empty))
            {
                new_custom_code = GenerateNewCode();
            }
            else if (IsValideCustomCode(custom_code) == false)
            {
                throw new Exception("Código informado indisponível!");
            }
            else
            {
                return custom_code;
            }

            return new_custom_code;
        }

        public async Task<UrlCreatedResponse> CreateUrlAsync(UrlData url_data)
        {
            string customCode = GenerateCustomCode(url_data.customCode);

            Url url = new()
            {
                code = customCode,
                original_url = url_data.url,
                click_count = 0
            };

            await _db.Urls.AddAsync(url);
            await _db.SaveChangesAsync();

            return new UrlCreatedResponse
            {
                shortenedUrl = customCode
            };
        }

        public async Task<GetUrlResponse> GetUrlAsync(string code)
        {
            
            var url = await _db.Urls.FirstOrDefaultAsync(url => url.code == code) ?? throw new Exception("URL não encontrada!");
            url.click_count++;
            await _db.SaveChangesAsync();
            return new GetUrlResponse
            {
                originalUrl = url.original_url,
                accessCount = url.click_count
            };
            
        }

        public async Task<GetAllUrlsResponse[]> GetAllUrlsAsync()
        {
            var urls = await _db.Urls.ToListAsync();

            GetAllUrlsResponse[] response = urls.Select(url => new GetAllUrlsResponse
            {
                url_data = new UrlDataResponse
                {
                    code = url.code,
                    original_url = url.original_url,
                    created_at = url.created_at,
                    expires_at = url.expires_at,
                    click_count = url.click_count
                }
            }).ToArray();

            return response;
        }
    }
}
