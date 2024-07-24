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
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d558be8089ad4bd3a603d4c88620d4c3");
    }

    public async Task<string> Translate(string text, string lang)
    {
        var uri = "https://translation-api.ghananlp.org/v1/translate";

        var requestBody = new
        {
            @in = text,  // '@' is used to escape the reserved keyword 'in'
            lang = lang
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);

        using (var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json"))
        {
            var response = await _httpClient.PostAsync(uri, content);

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
                if (translation != null)
                {
                    return translation.ToString();
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
}
