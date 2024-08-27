using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Http;
using TALKPOLL.Models;

[ApiController]
[Route("api/[controller]")]
public class TranslationController : Controller
    {

    private readonly HttpClient _httpClient;

    public TranslationController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d558be8089ad4bd3a603d4c88620d4c3");
    }

    public async Task<string> Translate(string text, string lang)
    {
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d558be8089ad4bd3a603d4c88620d4c3");
        var uri = "https://translation-api.ghananlp.org/v1/translate";

        var requestBody = new
        {
            @in = text,
            lang ="en-"+lang
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);

        HttpResponseMessage response;

        using (var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json"))
        {
            response = await _httpClient.PostAsync(uri, content);
        }

        if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic translation;

                try
                {
                    translation = JsonConvert.DeserializeObject<dynamic>(responseBody);
                }
                catch (JsonException ex)
                {
                    throw new Exception("Error parsing translation response: " + ex.Message);
                }

                if (translation != null && translation.translation != null)
                {
                    return (string)translation.translation;
                }
                else
                {
                    throw new Exception("Translation response is missing the expected 'translation' field.");
                }
            }
            else
            {
                throw new Exception("Translation error: " + response.StatusCode);
            }
        }
    }


    

