using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using TALKPOLL.Models;

public class SurveyController : Controller
{
    

    private readonly TranslationService _translationService;

    public SurveyController(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task<IActionResult> Index(int id, string selectedLanguage)
{
    string title = HttpContext.Session.GetString("Title");
    string survey = HttpContext.Session.GetString("Survey");
    string ID = HttpContext.Session.GetString("UserID");

    ViewBag.SId = id;
    ViewBag.Id = ID;
    ViewBag.Title = title;
    ViewBag.Survey = survey;

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

        if (!string.IsNullOrEmpty(selectedLanguage) && selectedLanguage != "en")
        {
            foreach (var question in questionsList)
            {
                // Check if the translation exists in the database
                var existingQuestionTranslation = await GetTranslationAsync("Question", id, Convert.ToInt32(question.questionId), null, selectedLanguage);
                if (existingQuestionTranslation != null)
                {
                    question.text = existingQuestionTranslation;
                }
                else
                {
                    // Translate the question text
                    var translatedQuestionText = await _translationService.Translate(question.text, selectedLanguage);
                    question.text = translatedQuestionText;

                    // Insert translated question into the Translation table
                    await InsertTranslationAsync("Question", Convert.ToInt32(question.questionId), selectedLanguage, translatedQuestionText, id);
                }

                // Translate the options text
                foreach (var option in question.Options)
                {
                    // Check if the translation exists in the database
                    var existingOptionTranslation = await GetTranslationAsync("Option", id, null, Convert.ToInt32(option.optionId), selectedLanguage);
                    if (existingOptionTranslation != null)
                    {
                        option.text = existingOptionTranslation;
                    }
                    else
                    {
                        var translatedOptionText = await _translationService.Translate(option.text, selectedLanguage);
                        option.text = translatedOptionText;

                        // Insert translated option into the Translation table
                        await InsertTranslationAsync("Option", Convert.ToInt32(option.optionId), selectedLanguage, translatedOptionText, id);
                    }
                }
            }
        }

        return View(questionsList);
    }
}

private async Task<string> GetTranslationAsync(string resourceType, int surveyId, int? questionId, int? optionId, string languageCode)
{
    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        {
        string query = @"
            SELECT translatedText
            FROM Translation
            WHERE ComputedID = @ComputedID AND LanguageCode=@languageCode";

        // Construct the ComputedID
        string computedId = (questionId.HasValue ? questionId.Value.ToString() : string.Empty) + "_" +
                             (optionId.HasValue ? optionId.Value.ToString() : string.Empty);

        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ComputedID", computedId);
        command.Parameters.AddWithValue("@LanguageCode", languageCode);

        connection.Open();
        var result = await command.ExecuteScalarAsync();
        connection.Close();

        return result?.ToString();
    }
    }
    }


    private async Task InsertTranslationAsync(string resourceType, int resourceId, string languageCode, string translatedText, int id)
    {
    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = @"
            INSERT INTO Translation (languageCode, resourceType, surveyId, questionId, OptionId, TranslatedText)
            VALUES (@LanguageCode, @ResourceType, @SurveyId, @QuestionId, @OptionId, @TranslatedText)";

        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@LanguageCode", languageCode);
        command.Parameters.AddWithValue("@ResourceType", resourceType);

        // Set parameters based on the resource type
        if (resourceType == "Question")
        {
            command.Parameters.AddWithValue("@SurveyId", id);
            command.Parameters.AddWithValue("@QuestionId", resourceId);
            command.Parameters.AddWithValue("@OptionId", DBNull.Value);
        }
        else if (resourceType == "Option")
        {
            command.Parameters.AddWithValue("@SurveyId", id);
            command.Parameters.AddWithValue("@QuestionId", DBNull.Value);
            command.Parameters.AddWithValue("@OptionId", resourceId);
        }
        else
        {
            throw new ArgumentException("Invalid resource type");
        }

        command.Parameters.AddWithValue("@TranslatedText", translatedText);

        connection.Open();
        await command.ExecuteNonQueryAsync();
        connection.Close();
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

                    HttpContext.Session.SetString("Title", survey.title);
                     HttpContext.Session.SetString("Survey", "Survey");

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
