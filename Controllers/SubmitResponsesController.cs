using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SubmitResponsesController : Controller
{
    [HttpPost]
    [Route("api/response/submit")]
    public async Task<IActionResult> SubmitResponses(List<SurveyResponseDTO> responses)
    {
        string userId = HttpContext.Session.GetString("Userid");

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
                foreach (var answer in response.Answers)
                {
                    string query = @"
                        INSERT INTO Response (surveyId, userid, questionId, text, selectedOptionId, responseDate)
                        VALUES (@SurveyId, @UserId, @QuestionId, @Text, @SelectedOptionId, GETDATE())";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SurveyId", response.SurveyId);
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@QuestionId", response.QuestionId);

                        if (!string.IsNullOrEmpty(answer.Text))
                        {
                            command.Parameters.AddWithValue("@Text", answer.Text);
                            command.Parameters.AddWithValue("@SelectedOptionId", DBNull.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Text", DBNull.Value);
                            command.Parameters.AddWithValue("@SelectedOptionId", answer.SelectedOptionId ?? (object)DBNull.Value);
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        return Ok(new { message = "Responses submitted successfully" });
    }
}

public class SurveyResponseDTO
{
    public int SurveyId { get; set; }
    public int QuestionId { get; set; }
    public List<AnswerDTO> Answers { get; set; }
}

public class AnswerDTO
{
    public string Text { get; set; }
    public int? SelectedOptionId { get; set; }
}
