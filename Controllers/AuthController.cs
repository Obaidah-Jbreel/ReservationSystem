

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // for password hashing
using ReservationSystem.Models;
using ReservationSystem.Database;
using System.Linq;
namespace ReservationSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Register page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register page
        [HttpPost]
        public IActionResult Register(User user, string RePassword)
        {
            if (ModelState.IsValid)
            {
                // Check if the username or email already exists
                var existingUser = _context.User
                    .FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);

                if (existingUser != null)
                {
                    ViewBag.Error = "Username or Email already exists.";
                    return View();
                }

                // Check if passwords match
                if (user.Password != RePassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View();
                }

                // Hash the password
                var hashedPassword = _passwordHasher.HashPassword(null, user.Password);

                // Set the user's IP address
                user.IP = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Save the new user to the database
                user.Password = hashedPassword;
                _context.User.Add(user);
                _context.SaveChanges();

                // Redirect to login page after successful registration
                return RedirectToAction("Login");
            }

            return View();
        }

        // GET: Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login page
        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.User.FirstOrDefault(u => u.Username == user.Username);

            if (existingUser != null)
            {
                // Verify the password using the hashed password stored in the Password field
                var result = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Set the logged-in user in TempData to be accessed in the Welcome page
                    TempData["Username"] = existingUser.Username;

                    // Redirect to the Welcome page if the login is successful
                    return RedirectToAction("Welcome");
                }
            }

            // If login fails, return the error message and stay on the login page
            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        // Welcome page for logged-in users
        public IActionResult Welcome()
        {
            // Check if the user is logged in (if TempData["Username"] exists)
            if (TempData["Username"] != null)
            {
                // Get the username from TempData
                ViewBag.Username = TempData["Username"].ToString();
                return View();
            }
            else
            {
                // If not logged in, redirect to the login page
                return RedirectToAction("Login");
            }
        }
    }
}

