using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaimSystem.Models
{
    public class ClaimVerification
    {
        [Key]
        public int VerificationId { get; set; }

        // Foreign key to the Claim being verified
        [Required]
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public LecturerClaim Claim { get; set; }

        // The user (coordinator) who verified the claim
        [Required]
        [StringLength(100)]
        public string VerifiedBy { get; set; }

        // The date and time when verification occurred
        public DateTime VerifiedOn { get; set; } = DateTime.Now;

        // Verification result: Approved / Rejected / Pending
        [Required]
        public ClaimVerificationStatus Status { get; set; }

        // Optional notes or reason for rejection
        [StringLength(500)]
        public string Comments { get; set; }
    }

    public enum ClaimVerificationStatus
    {
        Pending = 0,
        Verified = 1,
        Rejected = 2
    }
}