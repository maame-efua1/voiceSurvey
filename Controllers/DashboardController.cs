using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
using Newtonsoft.Json;


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

        HttpContext.Session.SetString("Userid", Register.userid);
        HttpContext.Session.SetString("FirstName", Register.firstname);
        HttpContext.Session.SetString("LastName", Register.lastname);
        HttpContext.Session.SetString("Phonenumber", Register.phonenumber);
        HttpContext.Session.SetString("Gender", Register.gender);
        HttpContext.Session.SetString("DateOfBirth", Register.date_of_birth.ToString("yyyy-MM-dd"));
        
        return View(Register);
    }

    public IActionResult Surveylist()
    {
        string userId = HttpContext.Session.GetString("Userid");
        string connectionString = "Server=ANTOINETTE;Database=SurveyApp;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
        
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT S.* FROM Survey S LEFT JOIN Response R ON S.surveyId = R.surveyId AND R.userid = @userId "+ 
                           "WHERE S.status = 'Active' AND R.userid IS NULL;";

            SqlCommand command = new SqlCommand(query, connection);
            
           command.Parameters.AddWithValue("@userId", userId);

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

            
            string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            return View(surveyList);
    }

    [HttpPost]
    public IActionResult Edit([FromForm] Register user)
    {
    if (!ModelState.IsValid)
    {
        return Json(new { success = false, message = "Invalid input." });
    }

    string userId = HttpContext.Session.GetString("Userid");
    string connectionString = "Server=ANTOINETTE;Database=SurveyApp;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    // Extract the last nine digits of the phone number
    string lastNineDigits = user.phonenumber.Length > 9
        ? user.phonenumber.Substring(user.phonenumber.Length - 9)
        : user.phonenumber;

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = @"UPDATE [User] SET 
                            firstname = @firstname,
                            lastname = @lastname,
                            gender = @gender,
                            date_of_birth = @date_of_birth,
                            phonenumber = @phonenumber
                         WHERE userid = @userId";

        SqlCommand command = new SqlCommand(query, connection);

        // Use parameterized queries to prevent SQL injection
        command.Parameters.AddWithValue("@firstname", user.firstname);
        command.Parameters.AddWithValue("@lastname", user.lastname);
        command.Parameters.AddWithValue("@gender", user.gender);
        command.Parameters.AddWithValue("@date_of_birth", user.date_of_birth.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@phonenumber", lastNineDigits);
        command.Parameters.AddWithValue("@userId", userId);

        try
        {
            connection.Open();
            command.ExecuteNonQuery();

            // Return a JSON response indicating success
            return Json(new { success = true });
        }
        catch (SqlException ex)
        {
            // Log the error (add error logging functionality as needed)
            Console.WriteLine("SQL Error: " + ex.Message);
            return Json(new { success = false, message = "Database error. Please try again later." });
        }
        finally
        {
            connection.Close();
        }
        }
        return View();
      }

        public IActionResult Security()
    {
        string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            
        return View();
    }

    public IActionResult DeleteUser()
    {
        string userId = HttpContext.Session.GetString("Userid");
    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        
        string query = @"DELETE from [User] WHERE userid = @userId";
        string query1 = @"DELETE from Responses WHERE userid = @userId";

        using (SqlCommand command = new SqlCommand(query1, connection))
        {
            command.Parameters.AddWithValue("@userId", userId);
            command.ExecuteNonQuery();
        }
    
        using (SqlCommand command1 = new SqlCommand(query, connection))
        {
            command1.Parameters.AddWithValue("@userId", userId);
            command1.ExecuteNonQuery();
        }
        
            connection.Close();
         
    }
         return RedirectToAction("Home", "Index");
    }
    
}
