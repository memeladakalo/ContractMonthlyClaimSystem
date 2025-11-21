using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ContractMonthlyClaimSystem.Models
{
    public class ManagerApproval
    {
        [Key]
        public int ApprovalId { get; set; }

        // Link to the claim being approved
        [Required]
        public int LecturerClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public LecturerClaim LecturerClaim { get; set; }

        // Manager’s username or full name
        [Required]
        [StringLength(100)]
        public string ApprovedBy { get; set; }

        // Date/time when approved or rejected
        public DateTime ApprovedOn { get; set; } = DateTime.Now;

        // Current approval status: Pending, Approved, or Rejected
        [Required]
        public ManagerApprovalStatus Status { get; set; } = ManagerApprovalStatus.Pending;

        // Optional manager notes or rejection reason
        [StringLength(500)]
        public string Comments { get; set; }
    }

    public enum ManagerApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}