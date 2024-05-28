using Microsoft.AspNetCore.Mvc;
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
}
