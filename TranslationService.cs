using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Http;
using TALKPOLL.Models;
using Microsoft.Net.Http.Headers;
public class TranslationService
{
    private readonly HttpClient _httpClient;

    public TranslationService(HttpClient httpClient)
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
            var translation = JsonConvert.DeserializeObject<dynamic>(responseBody);
            return (string)translation.translation;
        }
        else
        {
            throw new Exception("Translation error: " + response.StatusCode);
        }
    }
}
