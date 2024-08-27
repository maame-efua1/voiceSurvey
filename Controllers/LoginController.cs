using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using TALKPOLL.Models;
using Microsoft.AspNetCore.Authentication;


public class LoginController : Controller
{
    private readonly IPasswordHasher<Register> _passwordHasher;

    public LoginController(IPasswordHasher<Register> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public IActionResult Index(Register user)
{
    if (!string.IsNullOrEmpty(user.email) && !string.IsNullOrEmpty(user.password))
    {
        string connectionString = "Server=LAPTOP-LIL017KH\\SQLEXPRESS;Database=TALKPOLL;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM [User] WHERE email = @email";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@email", user.email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hashedPassword = reader["password"].ToString();
                        
                        if (hashedPassword != null && _passwordHasher.VerifyHashedPassword(user, hashedPassword, user.password) == PasswordVerificationResult.Success)
                        {
                            HttpContext.Session.SetString("UserID", reader["userid"].ToString());
                            HttpContext.Session.SetString("FirstName", reader["firstname"].ToString());
                            HttpContext.Session.SetString("LastName", reader["lastname"].ToString());
                            HttpContext.Session.SetString("Gender", reader["gender"].ToString());
                            HttpContext.Session.SetString("DateOfBirth", reader["date_of_birth"].ToString());
                            HttpContext.Session.SetString("PhoneNumber", reader["phonenumber"].ToString());

                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid email or password");
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or password");
                        return View();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View();
            }
            finally
            {
                connection.Close();
            }
        }
    }
    return View();
}

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction("Index", "Login");
    }

}
