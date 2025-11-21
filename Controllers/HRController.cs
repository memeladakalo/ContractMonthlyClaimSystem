using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly AppDbContext _context;

        public HRController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------
        // VIEW ALL LECTURERS AND CLAIMS
        // -------------------------------------------------------
        public IActionResult Dashboard()
        {
            var claims = _context.Claims
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();
            return View(claims);
        }

        // -------------------------------------------------------
        // FILTER CLAIMS
        // -------------------------------------------------------
        [HttpPost]
        public IActionResult Filter(string lecturerName, string status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Claims.AsQueryable();

            if (!string.IsNullOrEmpty(lecturerName))
                query = query.Where(c => c.LecturerName.Contains(lecturerName));

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(status, out ClaimStatus claimStatus))
                    query = query.Where(c => c.Status == claimStatus);
            }

            if (fromDate.HasValue)
                query = query.Where(c => c.DateSubmitted >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(c => c.DateSubmitted <= toDate.Value);

            return View("Dashboard", query.ToList());
        }

        // -------------------------------------------------------
        // UPDATE LECTURER DATA
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult EditLecturer(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLecturer(LecturerClaim updatedClaim)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == updatedClaim.ClaimId);
            if (claim == null) return NotFound();

            claim.LecturerName = updatedClaim.LecturerName;
            claim.HoursWorked = updatedClaim.HoursWorked;
            claim.HourlyRate = updatedClaim.HourlyRate;
            claim.Notes = updatedClaim.Notes;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Lecturer claim updated successfully.";
            return RedirectToAction("Dashboard");
        }

        // -------------------------------------------------------
        // GENERATE PDF INVOICE USING iTextSharp
        // -------------------------------------------------------
        public IActionResult GenerateInvoice(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new Paragraph($"Invoice for Claim #{claim.ClaimId}"));
                doc.Add(new Paragraph($"Lecturer: {claim.LecturerName}"));
                doc.Add(new Paragraph($"Date: {claim.DateSubmitted.ToShortDateString()}"));
                doc.Add(new Paragraph($"Hours Worked: {claim.HoursWorked}"));
                doc.Add(new Paragraph($"Hourly Rate: R{claim.HourlyRate}"));
                doc.Add(new Paragraph($"Total Amount: R{claim.Total}"));
                doc.Add(new Paragraph("---------------------------------------------------"));
                doc.Add(new Paragraph("Thank you for your service."));

                doc.Close();
                byte[] fileBytes = ms.ToArray();
                return File(fileBytes, "application/pdf", $"Invoice_Claim_{claim.ClaimId}.pdf");
            }
        }

        // -------------------------------------------------------
        // SSRS REPORT (Optional)
        // -------------------------------------------------------
        public IActionResult GenerateReport()
        {
            // Placeholder: SSRS integration (RDLC) or LINQ-based reporting
            var claims = _context.Claims.ToList();
            return View(claims); // Can be passed to SSRS/RDL for report rendering
        }
    }
}
