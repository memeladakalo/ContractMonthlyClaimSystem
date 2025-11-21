using System;
using System.Linq;
using System.Threading.Tasks;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ContractMonthlyClaimSystem.Services
{
    [Authorize(Roles = "Manager")]
    public class ManagersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ManagersController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // -------------------------------------------------------
        // MANAGER DASHBOARD: View all claims
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult ReviewClaims()
        {
            // Show all claims with current status for manager view
            var claims = _context.Claims.ToList();
            return View(claims);
        }

        // -------------------------------------------------------
        // FINAL APPROVAL
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalApprove(int id)
        {
            try
            {
                var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(ReviewClaims));
                }

                if (claim.Status != ClaimStatus.Verified)
                {
                    TempData["InfoMessage"] = "Only VERIFIED claims can be approved by Manager.";
                    return RedirectToAction(nameof(ReviewClaims));
                }

                claim.Status = ClaimStatus.Approved;
                _context.SaveChanges();

                // ----------------------------
                // SEND EMAIL NOTIFICATION
                // ----------------------------
                if (!string.IsNullOrEmpty(claim.LecturerName)) // Replace with email property if available
                {
                    string lecturerEmail = "lecturer@example.com"; // TODO: Replace with actual lecturer email from DB
                    string subject = "Your claim has been approved by Manager";
                    string message = $"Hello {claim.LecturerName},<br>Your claim (ID: {claim.ClaimId}) has been approved by the Manager.";

                    await _emailService.SendEmailAsync(lecturerEmail, subject, message);
                }

                TempData["SuccessMessage"] = "Claim APPROVED by Manager successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(ReviewClaims));
        }

        // -------------------------------------------------------
        // FINAL REJECTION
        // -------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalReject(int id)
        {
            try
            {
                var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["ErrorMessage"] = "Claim not found.";
                    return RedirectToAction(nameof(ReviewClaims));
                }

                if (claim.Status == ClaimStatus.Approved)
                {
                    TempData["InfoMessage"] = "Approved claims cannot be rejected.";
                    return RedirectToAction(nameof(ReviewClaims));
                }

                claim.Status = ClaimStatus.Rejected;
                _context.SaveChanges();

                // ----------------------------
                // SEND EMAIL NOTIFICATION
                // ----------------------------
                if (!string.IsNullOrEmpty(claim.LecturerName)) // Replace with email property if available
                {
                    string lecturerEmail = "lecturer@example.com"; // TODO: Replace with actual lecturer email from DB
                    string subject = "Your claim has been rejected by Manager";
                    string message = $"Hello {claim.LecturerName},<br>Your claim (ID: {claim.ClaimId}) has been rejected by the Manager.";

                    await _emailService.SendEmailAsync(lecturerEmail, subject, message);
                }

                TempData["SuccessMessage"] = "Claim rejected by Manager.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(ReviewClaims));
        }
    }
}
