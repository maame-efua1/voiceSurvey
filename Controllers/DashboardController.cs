using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;


public class DashboardController : Controller
{
    public IActionResult Index()
    {
        var Register = new Register
        {
            userid = HttpContext.Session.GetString("UserID"),
            firstname = HttpContext.Session.GetString("FirstName"),
            lastname = HttpContext.Session.GetString("LastName"),
            phonenumber = HttpContext.Session.GetString("PhoneNumber"),
            gender = HttpContext.Session.GetString("Gender"),
            date_of_birth = DateTime.Parse(HttpContext.Session.GetString("DateOfBirth"))
        };

        return View(Register);
    }

    public IActionResult Surveylist()
    {
       
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Survey WHERE status='Active' ";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            List<Survey> surveyList = new List<Survey>();


            while (reader.Read())
            {
                Survey surveys = new Survey();
                surveys.surveyId = reader["surveyId"].ToString();
                surveys.title = reader["title"].ToString(); 

                surveyList.Add(surveys);
            }
            connection.Close();

            return View(surveyList);
    }
}
