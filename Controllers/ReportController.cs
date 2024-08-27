using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;

public class ReportController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Users(Register user)
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
        
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM [User]";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            List<Register> userlist = new List<Register>();


            while (reader.Read())
            {
                Register users = new Register();
                users.userid = reader["userid"].ToString();
                users.firstname = reader["firstname"].ToString();
                users.lastname = reader["lastname"].ToString();
                users.email = reader["email"].ToString();
                users.gender = reader["gender"].ToString();
                users.phonenumber = reader["phonenumber"].ToString(); 

                userlist.Add(users);
            }
            connection.Close();


            return View(userlist);
    }

    public IActionResult DeleteUser(string userId)
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

            SqlConnection connection = new SqlConnection(connectionString);

            string query = $"delete from [User] where userid={userId}";

            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("Users");
    }

    public IActionResult Surveys()
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
        
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT s.surveyId, s.title, CONCAT(u.firstname, ' ', u.lastname) AS creatorName, s.status,(SELECT COUNT(*) "+
                           "FROM Response r WHERE r.surveyId = s.surveyId) AS responses " +
                       "FROM Survey s JOIN [User] u ON s.creatorId = u.userid ";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            List<Survey> surveylist = new List<Survey>();


            while (reader.Read())
            {
                Survey survey = new Survey();
                survey.surveyId= reader["surveyId"].ToString();
                survey.title = reader["title"].ToString();
                survey.creatorName = reader["creatorName"].ToString();
                survey.status = reader["status"].ToString();
                survey.responses = reader["responses"].ToString();

                surveylist.Add(survey);
            }
            connection.Close();


            return View(surveylist);
    }

    public IActionResult DeleteSurvey(string surveyId)
{
    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();

        string deleteOptionsQuery = "DELETE FROM [Option] WHERE questionId IN (SELECT q.questionId FROM Question q WHERE q.surveyId = @surveyId)";
        string deleteQuestionsQuery = "DELETE FROM Question WHERE surveyId = @surveyId";
        string deleteSurveyQuery = "DELETE FROM Survey WHERE surveyId = @surveyId";
        string deleteTranslationQuery = "DELETE FROM Translation WHERE surveyId = @surveyId";
        
        using (SqlCommand command = new SqlCommand(deleteTranslationQuery, connection))
        {
            command.Parameters.AddWithValue("@surveyId", surveyId);
            command.ExecuteNonQuery();
        }

        using (SqlCommand command1 = new SqlCommand(deleteOptionsQuery, connection))
        {
            command1.Parameters.AddWithValue("@surveyId", surveyId);
            command1.ExecuteNonQuery();
        }

        using (SqlCommand command2 = new SqlCommand(deleteQuestionsQuery, connection))
        {
            command2.Parameters.AddWithValue("@surveyId", surveyId);
            command2.ExecuteNonQuery();
        }

        using (SqlCommand command3 = new SqlCommand(deleteSurveyQuery, connection))
        {
            command3.Parameters.AddWithValue("@surveyId", surveyId);
            command3.ExecuteNonQuery();
        }

        connection.Close();
    }

            return RedirectToAction("Surveys");
    }

    public IActionResult Responses(Register user)
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
        
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM [User]";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            List<Register> userlist = new List<Register>();


            while (reader.Read())
            {
                Register users = new Register();
                users.userid = reader["userid"].ToString();
                users.firstname = reader["firstname"].ToString();
                users.lastname = reader["lastname"].ToString();
                users.email = reader["email"].ToString();
                users.gender = reader["gender"].ToString();
                users.phonenumber = reader["phonenumber"].ToString(); 

                userlist.Add(users);
            }
            connection.Close();


            return View(userlist);
    }

    public IActionResult DeleteResponse(string userId)
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

            SqlConnection connection = new SqlConnection(connectionString);

            string query = $"delete from [User] where userid={userId};";

            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("Users");
    }
}
