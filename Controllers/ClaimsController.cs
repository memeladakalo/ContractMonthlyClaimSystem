using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ClaimsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Display all claims
        [HttpGet]
        public IActionResult Index()
        {
            var claims = _context.Claims.ToList();
            return View(claims);
        }

        // GET: Create claim page
        [HttpGet]
        public IActionResult Create()
        {
            return View(new LecturerClaim());
        }

        // POST: Create a new claim with file upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LecturerClaim claim, IFormFile? LecturerDocument)
        {
            if (!ModelState.IsValid)
                return View(claim);

            try
            {
                // Handle supporting document upload
                if (LecturerDocument != null && LecturerDocument.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(LecturerDocument.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await LecturerDocument.CopyToAsync(stream);

                    claim.LecturerDocumentPath = "/uploads/" + uniqueFileName;
                }

                // Set default values
                claim.Status = ClaimStatus.Submitted;
                claim.DateSubmitted = DateTime.Now;

                _context.Claims.Add(claim);
               await _context.SaveChangesAsync(); //Synchronous

                TempData["SuccessMessage"] = "✅ Claim submitted successfully!";
                return RedirectToAction(nameof(Success));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ An error occurred: {ex.Message}";
                return View(claim);
            }
        }

        // GET: Claim submission success page
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        // GET: Claim details
        [HttpGet]
        public IActionResult Details(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "❌ Claim not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(claim);
        }

        // GET: Delete confirmation page
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "❌ Claim not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(claim);
        }

        // POST: Confirm deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                _context.Claims.Remove(claim);
                _context.SaveChangesAsync();
                TempData["InfoMessage"] = "🗑️ Claim deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Claim not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
