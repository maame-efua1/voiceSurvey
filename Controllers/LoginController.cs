using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;

public class LoginController : Controller
{
    private readonly IPasswordHasher<Register> _passwordHasher;

    public LoginController(IPasswordHasher<Register> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public IActionResult Index(Register user)
    {
        // Check if the user submitted the login form
        if (!string.IsNullOrEmpty(user.email) && !string.IsNullOrEmpty(user.password))
        {
            string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT password, userid, firstname, lastname, email, gender, date_of_birth FROM [User] WHERE email = @email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", user.email);

                try
                {
                    connection.Open();
                    string hashedPassword = (string)command.ExecuteScalar();

                    // Verify the provided password against the hashed password retrieved from the database
                    if (hashedPassword != null && _passwordHasher.VerifyHashedPassword(user, hashedPassword, user.password) == PasswordVerificationResult.Success)
                    {
                        // Passwords match, user is authenticated
                        // Redirect to the dashboard or perform any other action
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                    {
                        // Retrieve user details from the database
                      //  int userId = reader.GetInt32(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);
                        string email = reader.GetString(3);
                        string gender = reader.GetString(4);
                        DateTime dateOfBirth = reader.GetDateTime(5);

                        // Create a new User object with retrieved details
                        Register authenticatedUser = new Register
                        {
                          //  userid = userId,
                            firstname= firstName,
                            lastname = lastName,
                            email = email,
                            gender = gender,
                            date_of_birth= dateOfBirth
                        };

                        return RedirectToAction("Index", "Dashboard", authenticatedUser);
                    }
                    else
                    {
                        // Passwords don't match, display error message
                        ModelState.AddModelError(string.Empty, "Invalid email or password");
                        return View();
                    }
                }
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
