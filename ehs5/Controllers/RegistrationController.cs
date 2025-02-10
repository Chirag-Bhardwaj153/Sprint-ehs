using Microsoft.AspNetCore.Mvc;
using ehs5.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ehs5.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly Ehs1Context _context;

        public RegistrationController(Ehs1Context context)
        {
            _context = context;
        }

        // GET: Register
        // GET: User/Register
        public IActionResult Register()
        {
            // Populate ViewBag with Cities and States for dropdowns
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.States = _context.States.ToList();
            ViewBag.UserTypes = new List<string> { "Seller", "Buyer" }; // Updated user types (Seller, Buyer)
            return View();
        }

        // POST: User/Register
        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password, string userType, int cityId, int stateId, string firstName, string lastName, string phoneNo, string emailId)
        {
            if (ModelState.IsValid)
            {
                // Check if the UserName already exists in the Users table
                var existingUser = _context.Users.FirstOrDefault(u => u.UserName == username);
                if (existingUser != null)
                {
                    // You can return an error or a message here if the user already exists.
                    ModelState.AddModelError("UserName", "This username is already taken.");
                    ViewBag.Cities = _context.Cities.ToList();
                    ViewBag.States = _context.States.ToList();
                    ViewBag.UserTypes = new List<string> { "Seller", "Buyer" };
                    return View();
                }

                // Create a new User and save to the database
                var user = new User
                {
                    UserName = username,
                    Password = password,  // Make sure to hash the password before saving
                    UserType = userType,
                    UserCityId = cityId,
                    UserStateId = stateId
                };

                _context.Users.Add(user);
                _context.SaveChanges();  // Save User first

                // Handle the registration based on the User Type (Seller or Buyer)
                if (userType == "Seller")
                {
                    // Create new Seller and save to the database
                    var seller = new Seller
                    {
                        UserName = username,  // Link the UserName
                        FirstName = firstName,
                        LastName = lastName,
                        PhoneNo = phoneNo,
                        EmailId = emailId,
                        SellerCityId = cityId,
                        SellerStateId = stateId,
                        Address= _context.Cities
    .Where(a => a.CityId == cityId)
    .FirstOrDefault()?.CityName+" "+_context.States.Where(a=>a.StateId==stateId).FirstOrDefault()?.StateName

                    };

                    _context.Sellers.Add(seller);
                    _context.SaveChanges();  // Save Seller
                }
                else if (userType == "Buyer")
                {
                    // Create new Buyer and save to the database
                    var buyer = new Buyer
                    {
                        UserName = username,  // Link the UserName
                        FirstName = firstName,
                        LastName = lastName,
                        PhoneNo = phoneNo,
                        EmailId = emailId,
                        BuyerCityId = cityId,
                        BuyerStateId = stateId
                    };

                    _context.Buyers.Add(buyer);
                    _context.SaveChanges();  // Save Buyer
                }

                return RedirectToAction("Index", "Home"); // Redirect to Home or any other page after successful registration
            }

            // If model state is invalid, return to Register view with existing data
            ViewBag.Cities = _context.Cities.ToList();
            ViewBag.States = _context.States.ToList();
            ViewBag.UserTypes = new List<string> { "Seller", "Buyer" };
            return View();
        }

    }
}
