using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using TALKPOLL.Models;

public class SurveyController : Controller
{
    
    /*public IActionResult Index()
        {
            var surveyData = GetSurveyData();
            return View(surveyData);
        }

        private List<SurveyQuestion> GetSurveyData()
        {
            var surveyQuestions = new List<SurveyQuestion>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string queryQuestions = "SELECT Id, Question FROM SurveyQuestions";
                using (var commandQuestions = new SqlCommand(queryQuestions, connection))
                {
                    using (var readerQuestions = commandQuestions.ExecuteReader())
                    {
                        while (readerQuestions.Read())
                        {
                            var surveyQuestion = new SurveyQuestion
                            {
                                Id = readerQuestions.GetInt32(0),
                                Question = readerQuestions.GetString(1),
                                Options = new List<SurveyOption>()
                            };

                            surveyQuestions.Add(surveyQuestion);
                        }
                    }
                }

                string queryOptions = "SELECT Id, OptionText, SurveyQuestionId FROM SurveyOptions";
                using (var commandOptions = new SqlCommand(queryOptions, connection))
                {
                    using (var readerOptions = commandOptions.ExecuteReader())
                    {
                        while (readerOptions.Read())
                        {
                            var surveyOption = new SurveyOption
                            {
                                Id = readerOptions.GetInt32(0),
                                OptionText = readerOptions.GetString(1),
                                SurveyQuestionId = readerOptions.GetInt32(2)
                            };

                            var question = surveyQuestions.Find(q => q.Id == surveyOption.SurveyQuestionId);
                            if (question != null)
                            {
                                question.Options.Add(surveyOption);
                            }
                        }
                    }
                }
            }

            return surveyQuestions;
        }*/
        public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int id)
{
    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = "SELECT s.surveyId, s.title, s.description, CONCAT(u.firstname, ' ', u.lastname) AS creatorName, s.dateCreated, s.expiryDate, s.status, s.language " +
                       "FROM Survey s JOIN [User] u ON s.creatorId = u.userid WHERE s.surveyId = @id";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Survey survey = new Survey
                    {
                        surveyId = reader["surveyId"].ToString(), // Adjust this based on the data type of surveyId in Survey class
                        title = reader["title"].ToString(),
                        description = reader["description"].ToString(),
                        creatorName = reader["creatorName"].ToString(),
                        dateCreated = Convert.ToDateTime(reader["dateCreated"]).ToString("dd-MM-yyyy"),
                        expiryDate = Convert.ToDateTime(reader["expiryDate"]).ToString("dd-MM-yyyy"),
                        status = reader["status"].ToString(),
                        language = reader["language"].ToString()
                    };

                    return View(survey);
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }
}

}
