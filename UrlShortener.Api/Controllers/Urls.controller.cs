using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortener.Api.Models;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("url")]
public class URLsController : ControllerBase
{
    private readonly Urls _urls;

    public URLsController(Urls urls)
    {
        _urls = urls;
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateUrlData(UrlData request)
    {
        try
        {
            return Ok(await _urls.CreateUrlAsync(request));
        } catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar a URL - {ex.Message}");
            return BadRequest(new {
                status_code = HttpStatusCode.BadRequest,
                error_message = $"Erro ao criar a URL - {ex.Message}"
            });
        }
    }

    [HttpGet("get/{code}")]
    public async Task<IActionResult> GetUrlData(string code)
    {
        try
        {
            return Ok(await _urls.GetUrlAsync(code));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar a URL - {ex.Message}");
            return BadRequest(new
            {
                status_code = HttpStatusCode.BadRequest,
                error_message = $"Erro ao buscar a URL - {ex.Message}"
            });
        }
    }
}
