using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturersController : Controller
    {
        
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LecturersController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //SUBMIT NEW CLAIM
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(LecturerClaim claim)
        {
            if (!ModelState.IsValid)
                return View(claim);

            try
            {
                // ---------------------------------------
                // Secure file upload
                // ---------------------------------------
                var file = Request.Form.Files.FirstOrDefault();

                if (file != null && file.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Only PDF, DOCX, and XLSX files are allowed.");
                        return View(claim);
                    }

                    if (file.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "File size cannot exceed 3MB.");
                        return View(claim);
                    }

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }

                    claim.LecturerDocumentPath = "/uploads/" + fileName;
                }

                // ---------------------------------------
                // Default claim status
                // ---------------------------------------
                claim.Status = ClaimStatus.Submitted;

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["LecturerName"] = claim.LecturerName;
                TempData["HoursWorked"] = claim.HoursWorked;
                TempData["HourlyRate"] = claim.HourlyRate;
                TempData["TotalAmount"] = claim.Total;

                return RedirectToAction("✅ClaimSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View(claim);
            }
        }

        private readonly IEmailService _email;

        public LecturersController(IEmailService email)
        {
            _email = email;
        }

        public async Task<IActionResult> NotifyCoordinator(int claimId)
        {
            await _email.SendEmailAsync(
                "coordinator@college.ac.za",
                "New Claim Submitted",
                $"A lecturer submitted a claim. Claim ID: {claimId}"
            );

            return Ok("Email Sent");
        }


        //claim submission page
        public IActionResult ClaimSuccess()
        {
            return View();
        }
        //SHOW LECTURER CLAIMS WITH PROGRESS BAR
        //OPTIONAL FILTERING BY STATUS, MONTH, ETC
        public IActionResult Index(string status = "", int? month = null)
        {
            var claims = _context.Claims.AsQueryable();

            //Only show claims submitted by the lecturer
            var currentUser = User.Identity.Name;
            claims = claims.Where(c => c.LecturerName == currentUser);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, out ClaimStatus parsedStatus))
                claims = claims.Where(c => c.Status == parsedStatus);

            if (month.HasValue)
                claims = claims.Where(c => c.DateSubmitted.Month == month.Value);

            return View(claims.ToList()); //Progress bar rendered inside razor view
        }
    }
}
