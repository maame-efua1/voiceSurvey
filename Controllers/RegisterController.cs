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
        //HttpContext.Session.SetString("firstname", "");

    string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
    
     // Create an instance of PasswordHasher
        var passwordHasher = new PasswordHasher<Register>();

        // Hash the password
        
        if (!string.IsNullOrEmpty(User.password))
        {
            // Hash the password using the injected PasswordHasher
            string hashedPassword = passwordHasher.HashPassword(User, User.password);
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO [User] (firstname, lastname, email, password, gender, date_of_birth, phonenumber) VALUES (@firstname, @lastname, @email, @password, @gender, @date_of_birth, @phonenumber)";

            SqlCommand command = new SqlCommand(query, connection);

            // Replace parameters with actual values from the User object
            command.Parameters.AddWithValue("@firstname", User.firstname);
            command.Parameters.AddWithValue("@lastname", User.lastname);
            command.Parameters.AddWithValue("@email", User.email);
            command.Parameters.AddWithValue("@password", hashedPassword);  // Use hashed password
            command.Parameters.AddWithValue("@gender", User.gender);
            command.Parameters.AddWithValue("@date_of_birth", User.date_of_birth);
            command.Parameters.AddWithValue("@phonenumber", User.phonenumber);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                
                return RedirectToAction("Index", "Login");
                
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
        }
    return View();
    }
}
