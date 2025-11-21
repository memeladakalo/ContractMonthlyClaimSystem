using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;

namespace YourApp.Controllers
{
    [Authorize(Roles = "Coordinator")] // Only coordinators can access
    public class ClaimVerificationController : Controller
    {
        private readonly AppDbContext _context;

        public ClaimVerificationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: List all claims
        public IActionResult Index()
        {
            var claims = _context.ClaimVerifications.ToList();
            return View(claims);
        }

        // GET: Verify a specific claim
        public IActionResult Verify(int id)
        {
            var claim = _context.ClaimVerifications.FirstOrDefault(c => c.VerificationId == id);
            if (claim == null) return NotFound();

            return View(claim);
        }

        // POST: Verify claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id, bool isVerified)
        {
            var claim = _context.ClaimVerifications.FirstOrDefault(c => c.VerificationId == id);
            if (claim == null) return NotFound();

            
            claim.VerifiedBy = User.Identity.Name; // current coordinator
            claim.VerifiedOn = System.DateTime.Now;

            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
