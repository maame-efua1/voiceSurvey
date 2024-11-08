using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class TTSController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public TTSController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("speak")]
    public async Task<IActionResult> Speak([FromBody] SurveyData data)
    {
         
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "942881ecfe4a449b85bc1dc319ff45c9");
        var uri = "https://translation-api.ghananlp.org/tts/v1/tts";

        var requestBody = new
        {
            text = data.Text,
        };

       var jsonRequestBody = JsonConvert.SerializeObject(requestBody); 
        var content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(uri, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            return File(responseContent, "audio/mpeg");
        }
        else
        {
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
    }

    public class SurveyData
    {
        public string Text { get; set; }
    }
}
