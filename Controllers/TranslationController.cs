using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Http;
using TALKPOLL.Models;

[ApiController]
[Route("api/[controller]")]
public class TranslationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public TranslationController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("translate")]
    public async Task<ActionResult<TranslationResponse>> Translate([FromBody] TranslationRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.In) || string.IsNullOrEmpty(request.Lang))
        {
            return BadRequest("Invalid request.");
        }

        var translatedText = await TranslateTextAsync(request.In, request.Lang);

        if (translatedText == null)
        {
            return StatusCode(500, "Translation service failed.");
        }

        var response = new TranslationResponse
        {
            TranslatedText = translatedText
        };

        return Ok(response);
    }

    private async Task<string> TranslateTextAsync(string text, string lang)
    {
        var client = _httpClientFactory.CreateClient();
        var requestContent = new
        {
            @in = text,
            lang = lang
        };

        // Correctly serialize the request content to a JSON string
        var jsonContent = JsonConvert.SerializeObject(requestContent);

        // Create the StringContent with the serialized JSON string
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        // Get the API key and base URL from configuration
        var apiKey = _configuration["TranslationService:ApiKey"];
        var baseUrl = _configuration["TranslationService:BaseUrl"];

        // Add the API key to the request headers
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // Post the request and get the response
        var response = await client.PostAsync($"{baseUrl}/translate", content);

        if (!response.IsSuccessStatusCode)
        {
          // Handle unsuccessful response
        var errorContent = await response.Content.ReadAsStringAsync();
        // Optionally log the error or handle it as needed
        return $"Error: {response.StatusCode}, {errorContent}";
        }

        // Read the response content as a string
        var responseContent = await response.Content.ReadAsStringAsync();

        // Deserialize the response content to a TranslationResponse object
        var translationResponse = JsonConvert.DeserializeObject<TranslationResponse>(responseContent);

        return translationResponse?.TranslatedText;
    }
}
