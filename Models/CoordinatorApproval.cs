using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ContractMonthlyClaimSystem.Models
{
    public class CoordinatorApproval
    {
        [Key]
        public int CoordinatorApprovalId { get; set; }

        // Foreign key connecting this approval record to a specific claim
        [ForeignKey("Claim")]
        public int LecturerClaimId { get; set; }
        public LecturerClaim Claim { get; set; }

        // The coordinator responsible for verification
        [Required]
        [StringLength(150)]
        public string CoordinatorName { get; set; }

        // Approval-related info
        public bool IsApproved { get; set; } = false;

        [StringLength(500)]
        public string Remarks { get; set; }

        // Metadata — dates and user who approved
        public DateTime? DateApproved { get; set; }
        public string ApprovedByUserId { get; set; } // Optional: tie back to ApplicationUser
    }
}