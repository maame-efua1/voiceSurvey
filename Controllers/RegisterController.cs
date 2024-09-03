using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
public class RegisterController : Controller
{
    private readonly IPasswordHasher<Register> _passwordHasher;

    public RegisterController(IPasswordHasher<Register> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public IActionResult Index(Register User)
    {

    string connectionString = "Server=ANTOINETTE;Database=SurveyApp;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
    
        var passwordHasher = new PasswordHasher<Register>();

        if (!string.IsNullOrEmpty(User.password))
        {
            string hashedPassword = passwordHasher.HashPassword(User, User.password);

        string lastNineDigits = User.phonenumber.Length > 9
            ? User.phonenumber.Substring(User.phonenumber.Length - 9)
            : User.phonenumber;
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO [User] (firstname, lastname, email, password, gender, date_of_birth, phonenumber, usertype) VALUES (@firstname, @lastname, @email, @password, @gender, @date_of_birth, @phonenumber, @usertype)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@firstname", User.firstname);
            command.Parameters.AddWithValue("@lastname", User.lastname);
            command.Parameters.AddWithValue("@email", User.email);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@gender", User.gender);
            command.Parameters.AddWithValue("@date_of_birth", User.date_of_birth);
            command.Parameters.AddWithValue("@phonenumber", lastNineDigits);
            command.Parameters.AddWithValue("@usertype", "2");

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                
                return RedirectToAction("Index", "Login");
                
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        }
        return View();
        }
}
