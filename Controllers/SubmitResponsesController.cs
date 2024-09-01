using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SubmitResponsesController : Controller
{
    [HttpPost]
    [Route("api/response/submit")]
    public async Task<IActionResult> SubmitResponses([FromBody] List<SurveyResponse> responses)
    {
        string userId = HttpContext.Session.GetString("Userid");
        string surveyId = HttpContext.Session.GetString("SID");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not logged in" });
        }

        string connectionString = "Server=ANTOINETTE;Database=SurveyApp;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (var response in responses)
            {
                string? selectedOptionId = response.Answer_Option; // Use Answer_Options if available, otherwise use Answer_Option

                    string query = @"
                        INSERT INTO Response (surveyId, userid, questionId, text, selectedOptionId, responseDate)
                        VALUES (@SurveyId, @UserId, @QuestionId, @Text, @SelectedOptionId, GETDATE())";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                    command.Parameters.AddWithValue("@SurveyId", surveyId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@QuestionId", response.QuestionId);
                    command.Parameters.AddWithValue("@Text", response.Answer_Text ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SelectedOptionId", selectedOptionId ??(object) DBNull.Value);


                        await command.ExecuteNonQueryAsync();
                    
                }
            }
        }

        return Ok(new { message = "Responses submitted successfully" });
    }
}

public class SurveyResponse
{
    public int QuestionId { get; set; }
    public string? Answer_Text { get; set; }
    public string? Answer_Option { get; set; }
   // public string? Answer_Options { get; set; }
}



