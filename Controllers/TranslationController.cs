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
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "e9e02d0c1c83456ab5a1ce5858190459");
    }

    public async Task<string> Translate(string text, string lang)
    {
        var uri = "https://translation-api.ghananlp.org/v1/translate";

        var requestBody = new
        {
            @in = text,
            lang = lang
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

                // Check if the 'translation' object and 'translation.translation' are not null
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


    

