using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;

[Route("api/[controller]")]
[ApiController]
public class TranscriptionController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public TranscriptionController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    
    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe([FromForm] IFormFile audio)
{
    string language = HttpContext.Session.GetString("selectedLanguage");
    var client = _clientFactory.CreateClient();
    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "942881ecfe4a449b85bc1dc319ff45c9");

    var uri = $"https://translation-api.ghananlp.org/asr/v1/transcribe?language={language}";

    using (var content = new MultipartFormDataContent())
    {
        var audioStream = new MemoryStream();
        await audio.CopyToAsync(audioStream);
        audioStream.Seek(0, SeekOrigin.Begin);

        content.Add(new StreamContent(audioStream), "audio", "audio.mpeg");
        // No need to set content.Headers.ContentType here

        var response = await client.PostAsync(uri, content);

        if (response.IsSuccessStatusCode)
        {
            var transcription = await response.Content.ReadAsStringAsync();
            return Ok(new { transcription });
        }
        else
        {
            return BadRequest("Transcription failed");
        }
    }
}

}
