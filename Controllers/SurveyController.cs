using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using TALKPOLL.Models;

public class SurveyController : Controller
{
     /*       public IActionResult Index()
    {

        return View();
    }*/

    private readonly TranslationService _translationService;

    public SurveyController(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task<IActionResult> Index(int id, string selectedLanguage)
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"
                SELECT 
                    q.questionId,
                    q.text AS question_text,
                    q.type AS question_type,
                    q.position AS question_position,
                    q.isRequired AS question_is_required,
                    o.optionId,
                    o.text AS option_text,
                    o.position AS option_position
                FROM 
                    Question q
                LEFT JOIN 
                    [Option] o ON q.questionId = o.questionId
                WHERE 
                    q.surveyId = @surveyId
                ORDER BY 
                    q.position, o.position";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@surveyId", id);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            List<SurveyQuestions> questionsList = new List<SurveyQuestions>();

            while (reader.Read())
            {
                string questionId = reader["questionId"].ToString();

                var question = questionsList.FirstOrDefault(q => q.questionId == questionId);
                if (question == null)
                {
                    question = new SurveyQuestions
                    {
                        questionId = questionId,
                        surveyId = id,
                        text = reader["question_text"] != DBNull.Value ? reader["question_text"].ToString() : string.Empty,
                        type = reader["question_type"] != DBNull.Value ? reader["question_type"].ToString() : string.Empty,
                        position = reader["question_position"] != DBNull.Value ? Convert.ToInt32(reader["question_position"]) : 0,
                        IsRequired = reader["question_is_required"] != DBNull.Value ? Convert.ToBoolean(reader["question_is_required"]) : false,
                        Options = new List<SurveyOptions>()
                    };
                    questionsList.Add(question);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("optionId")))
                {
                    var option = new SurveyOptions
                    {
                        optionId = reader["optionId"].ToString(),
                        text = reader["option_text"] != DBNull.Value ? reader["option_text"].ToString() : string.Empty,
                        position = reader["option_position"] != DBNull.Value ? Convert.ToInt32(reader["option_position"]) : 0
                    };
                    question.Options.Add(option);
                }
            }

            connection.Close();

            // Translate the questions and options if a language other than English is selected
            if (!string.IsNullOrEmpty(selectedLanguage) && selectedLanguage != "en")
            {
                foreach (var question in questionsList)
                {
                    // Translate the question text
                    question.text = await _translationService.Translate(question.text, selectedLanguage);

                    // Translate the options text
                    foreach (var option in question.Options)
                    {
                        option.text = await _translationService.Translate(option.text, selectedLanguage);
                    }
                }
            }

            ViewBag.QuestionsJson = JsonConvert.SerializeObject(questionsList);

            return View(questionsList);
        }
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
