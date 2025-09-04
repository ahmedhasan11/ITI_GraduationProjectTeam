using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ITI_Hackathon.Models;
using ITI_Hackathon.Models.Account;
using Microsoft.AspNetCore.Identity;
using ITI_Hackathon.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ITI_Hackathon.Entities;



namespace ITI_Hackathon.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }
        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("Doctor"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Doctor"));
            }
            if (!await _roleManager.RoleExistsAsync("Patient"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Patient"));
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                IsDoctor = model.Role == "Doctor",
                IsPatient = model.Role == "Patient"
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var e in createResult.Errors) ModelState.AddModelError("", e.Description);
                return View(model);
            }

            // Assign role
            await _userManager.AddToRoleAsync(user, model.Role);

            if (model.Role == "Doctor")
            {
                var doc = new DoctorProfile
                {
                    UserId = user.Id,
                    Specialty = "General",
                    IsApproved = false
                };
                _db.Doctors.Add(doc);
                await _db.SaveChangesAsync();

                // Do not auto-sign-in doctors (wait for admin approve)
                TempData["Message"] = "Registration successful. Your doctor account is pending admin approval.";
                return RedirectToAction(nameof(RegisterSuccess));
            }
            else
            {
                var patient = new PatientProfile
                {
                    UserId = user.Id
                };
                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();

                // sign in patient
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult RegisterSuccess()
        {
            return View(); // simple view tells user to wait for approval
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // If doctor, ensure approved by admin
            if (await _userManager.IsInRoleAsync(user, "Doctor"))
            {
                var doc = await _db.Doctors.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (doc == null || !doc.IsApproved)
                {
                    ModelState.AddModelError("", "Doctor account is not approved yet.");
                    return View(model);
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
