using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
using Newtonsoft.Json;

public class SubmitResponsesController : Controller
{
[HttpPost]
[Route("api/response/submit")]
public async Task<IActionResult> SubmitResponses([FromBody] List<SurveyResponse> responses)
{
    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        foreach (var response in responses)
        {
            string query = @"
                INSERT INTO Responses (surveyId, userid, questionId, text, selectedOptionId, responseDate)
                VALUES (@SurveyId, @UserId, @QuestionId, @Text, @SelectedOptionId, GETDATE())";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SurveyId", response.SurveyId);
                command.Parameters.AddWithValue("@UserId", response.UserId);
                command.Parameters.AddWithValue("@QuestionId", response.QuestionId);
                command.Parameters.AddWithValue("@Text", response.Text ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SelectedOptionId", response.SelectedOptionId ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }
        connection.Close();
    }

    return Ok(new { message = "Responses submitted successfully" });
}

public class SurveyResponse
{
    public int SurveyId { get; set; }
    public int UserId { get; set; }
    public int QuestionId { get; set; }
    public string Text { get; set; }
    public int? SelectedOptionId { get; set; }
}

}