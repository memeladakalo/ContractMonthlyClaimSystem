using System;
using System.Linq;
using System.Threading.Tasks;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using ContractMonthlyClaimSystem.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public CoordinatorsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // -------------------------------------------------------
        // SHOW ALL CLAIMS FOR REVIEW (supports progress bar)
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult VerifyClaims()
        {
            // Show ALL claims so progress bars reflect all statuses
            var claims = _context.Claims.ToList();
            return View(claims);
        }

        // -------------------------------------------------------
        // APPROVE (VERIFY) CLAIM
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            try
            {
                var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(VerifyClaims));
                }

                if (claim.Status != ClaimStatus.Submitted)
                {
                    TempData["InfoMessage"] = "Only submitted claims can be verified.";
                    return RedirectToAction(nameof(VerifyClaims));
                }

                // Update status
                claim.Status = ClaimStatus.Verified;
                _context.SaveChanges();

                // ----------------------------
                // SEND EMAIL NOTIFICATION
                // ----------------------------
                if (!string.IsNullOrEmpty(claim.LecturerName)) // Replace with email property if available
                {
                    string lecturerEmail = "lecturer@example.com"; // TODO: Replace with actual lecturer email from DB
                    string subject = "Your claim has been verified";
                    string message = $"Hello {claim.LecturerName},<br>Your claim (ID: {claim.ClaimId}) has been verified by the coordinator.";

                    await _emailService.SendEmailAsync(lecturerEmail, subject, message);
                }

                TempData["SuccessMessage"] = "Claim verified successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }

        // -------------------------------------------------------
        // REJECT CLAIM
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectClaim(int id)
        {
            try
            {
                var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(VerifyClaims));
                }

                if (claim.Status == ClaimStatus.Approved)
                {
                    TempData["InfoMessage"] = "Approved claims cannot be rejected.";
                    return RedirectToAction(nameof(VerifyClaims));
                }

                // Update status
                claim.Status = ClaimStatus.Rejected;
                _context.SaveChanges();

                // ----------------------------
                // SEND EMAIL NOTIFICATION
                // ----------------------------
                if (!string.IsNullOrEmpty(claim.LecturerName)) // Replace with email property if available
                {
                    string lecturerEmail = "lecturer@example.com"; // TODO: Replace with actual lecturer email from DB
                    string subject = "Your claim has been rejected";
                    string message = $"Hello {claim.LecturerName},<br>Your claim (ID: {claim.ClaimId}) has been rejected by the coordinator. Please contact the coordinator for details.";

                    await _emailService.SendEmailAsync(lecturerEmail, subject, message);
                }

                TempData["SuccessMessage"] = "Claim rejected successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(VerifyClaims));


           

           

        }
    }
}
