using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SubmitResponsesController : Controller
{
    [HttpPost]
    [Route("api/response/submit")]
    public async Task<IActionResult> SubmitResponses(List<SurveyResponse> responses)
    {
        string userId = HttpContext.Session.GetString("Userid");

        // Ensure the user is authenticated
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not logged in" });
        }

        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            foreach (var response in responses)
            {
                string query = @"
                    INSERT INTO Responses (surveyId, userid, questionId, text, selectedOptionId, responseDate)
                    VALUES (@SurveyId, @UserId, @QuestionId, @Text, @SelectedOptionId, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SurveyId", response.SurveyId);
                    command.Parameters.AddWithValue("@UserId", userId); // Use session userId
                    command.Parameters.AddWithValue("@QuestionId", response.QuestionId);
                    command.Parameters.AddWithValue("@Text", response.Text ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SelectedOptionId", response.SelectedOptionId ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        return Ok(new { message = "Responses submitted successfully" });
    }
}


