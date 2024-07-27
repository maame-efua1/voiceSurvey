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
         
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d558be8089ad4bd3a603d4c88620d4c3");
        var uri = "https://translation-api.ghananlp.org/tts/v1/tts";

        // Construct the request body
        var requestBody = new
        {
            text = data.Text,
            language = "tw" // Assuming the language is always "tw"
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
