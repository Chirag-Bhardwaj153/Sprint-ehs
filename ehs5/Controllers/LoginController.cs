using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ehs5.Auth;
using ehs5.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ehs5.Controllers
{
    public class LoginController : Controller
    {
        private readonly Ehs1Context _context;

        public LoginController(Ehs1Context context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([FromForm] Login collection)
        {
            try
            {
                // Fetch user from the database
                var user = await _context.Users
                    .Include(u => u.Buyers)  // Include Buyer if necessary for roles
                    .Include(u => u.Sellers) // Include Seller if necessary for roles
                    .FirstOrDefaultAsync(u => u.UserName == collection.UserName && u.Password == collection.Password);

                if (user == null)
                {
                    return BadRequest("Invalid username or password.");
                }

                // Role-based mapping logic
                List<KeyValuePair<string, string>> roles = new List<KeyValuePair<string, string>>();

                // Assign roles based on the UserType
                if (user.UserType == "Admin")
                {
                    roles.Add(new KeyValuePair<string, string>("Role", "Admin"));
                }
                else if (user.UserType == "User")
                {
                    roles.Add(new KeyValuePair<string, string>("Role", "User"));
                }
                else if (user.UserType == "Seller")
                {
                    roles.Add(new KeyValuePair<string, string>("Role", "Seller"));
                }
                else if (user.UserType == "Customer")
                {
                    roles.Add(new KeyValuePair<string, string>("Role", "Customer"));
                }

                // Generate JWT Token
                string token = GenerateToken.TokenGenerator("QW5hbmR2YXJtYXRlc3R2YXNybWEuYWpka2RrYWpramRmYWRzZg==", roles, 100);

                // Save token in session and set the header
                HttpContext.Session.SetString("JwtToken", $"Bearer {token}");
                HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

                // Redirect to the home page after login
                ViewBag.Token = token;
                return Redirect("/Home/Index");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error: {ex.Message}");
                return View();
            }
        }

    }
}
