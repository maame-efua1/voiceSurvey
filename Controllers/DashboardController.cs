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

            string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            return View(surveyList);
    }

    public IActionResult Edit(Register User)
    {
        string userId = HttpContext.Session.GetString("Userid");
        string firstName = HttpContext.Session.GetString("FirstName");
        string lastName = HttpContext.Session.GetString("LastName");
        string phonenumber = HttpContext.Session.GetString("Phonenumber");
        string gender = HttpContext.Session.GetString("Gender");
        string dob = HttpContext.Session.GetString("DateOfBirth");

        ViewBag.FirstName = firstName;
        ViewBag.LastName = lastName;
        ViewBag.PNumber = phonenumber;
        ViewBag.Gender = gender;
        ViewBag.DOB = dob;

        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
      
        string lastNineDigits = phonenumber.Length > 9
        ? phonenumber.Substring(phonenumber.Length - 9)
        : phonenumber;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"UPDATE [User] SET firstname = @firstname,
                    lastname = @lastname,
                    gender = @gender,
                date_of_birth = @date_of_birth,
                    phonenumber = @phonenumber
                 WHERE userid = @userid";


            SqlCommand command = new SqlCommand(query, connection);

            // Replace parameters with actual values from the User object
           command.Parameters.AddWithValue("@firstname", User.firstname);
            command.Parameters.AddWithValue("@lastname", User.lastname);  
            command.Parameters.AddWithValue("@gender", User.gender);
            command.Parameters.AddWithValue("@date_of_birth", User.date_of_birth.ToString());
            command.Parameters.AddWithValue("@phonenumber", lastNineDigits);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                
                return RedirectToAction("Index", "Dashboard");
                
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Handle the exception or provide feedback to the user
            }
            finally
            {
                connection.Close();
            }
        }

        return View();
    }
}
