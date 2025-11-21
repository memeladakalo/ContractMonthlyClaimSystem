using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _environment = environment;
        }

        // ----------------------------------------------------
        // LOGIN (GET)
        // ----------------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ----------------------------------------------------
        // LOGIN (POST)
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        // ----------------------------------------------------
        // LOGOUT
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ----------------------------------------------------
        // REGISTER (GET)
        // ----------------------------------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ----------------------------------------------------
        // REGISTER (POST)
        // ----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser user, string password, string role)
        {
            if (!ModelState.IsValid)
                return View(user);

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);

            return View(user);
        }

        // ----------------------------------------------------
        // SUBMIT CLAIM (Lecturer Action)
        // ----------------------------------------------------
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View(new LecturerClaim());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(LecturerClaim claim, IFormFile SupportingDocument)
        {
            if (!ModelState.IsValid)
                return View(claim);

            //------------------------------------------
            // FILE UPLOAD
            //------------------------------------------
            if (SupportingDocument != null && SupportingDocument.Length > 0)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(SupportingDocument.FileName)}";
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await SupportingDocument.CopyToAsync(fs);
                }

                claim.LecturerDocumentPath = $"/uploads/{fileName}";
            }

            //------------------------------------------
            // DEFAULT CLAIM STATUS
            //------------------------------------------
            claim.Status = ClaimStatus.Submitted;

            //------------------------------------------
            // SAVE CLAIM IN DATABASE
            //------------------------------------------
            _context.Set<LecturerClaim>().Add(claim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim submitted successfully!";
            return RedirectToAction("ClaimList", "Lecturers");
        }
    }
}
