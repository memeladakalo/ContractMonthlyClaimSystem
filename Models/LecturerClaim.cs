using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ContractMonthlyClaimSystem.Models
{
    // ✅ Enum for clear, maintainable status tracking
    public enum ClaimStatus
    {
        Pending,       // Claim has been submitted but not yet reviewed
        Submitted,     // Claim has been submitted ready to be reviewed 
        Verified,      // Checked and verified by coordinator
        Approved,      // Approved for payment by manager
        Rejected       // Rejected during review
    }

    public class LecturerClaim
    {
        [Key]
        public int LecturerClaimId { get; set; }
        //Foreign key to the main Claimtable
        public int ClaimId { get; set; }
        public LecturerClaim Claim { get; set; }

        [Required(ErrorMessage = "Lecturer name is required.")]
        [Display(Name = "Lecturer Name")]
        public string LecturerId { get; set; }
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime DateSubmitted { get; set; }

        [Required(ErrorMessage = "Hours worked must be between 1 and 200.")]
        [Range(1, 200, ErrorMessage = "Hours worked must be between 1 and 200.")]
        [Display(Name = "Hours Worked")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate must be between R100 and R2000.")]
        [Range(100, 2000, ErrorMessage = "Hourly rate must be between R100 and 2000.")]
        [Display(Name = "Hourly Rate (R)")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Supporting Document")]
        public string? LecturerDocumentPath { get; set; }

        // NEW: Claim status with default value
        [Display(Name = "Claim Status")]
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        // Calculated field (not mapped to DB)
        [NotMapped]
        [Display(Name = "Total Amount (R)")]
        public decimal Total => HoursWorked * HourlyRate;

        public string Description { get; set; } = string.Empty;

        public string Notes { get; set; }
        public CoordinatorApproval CoordinatorApproval { get; set; }
        public ManagerApproval ManagerApproval { get; set; }
        // ---------------------------
        // ✅ Progress bar HTML method
        // ---------------------------
        public string GetProgressBarHtml()
        {
            string status = Status.ToString();

            // Show RED progress bar if rejected
            if (Status == ClaimStatus.Rejected)
            {
                return @"
                <div class='progress mt-2'>
                    <div class='progress-bar bg-danger' style='width:100%'>
                        Rejected
                    </div>
                </div>";
            }

            // Percentage for each stage
            int percentage = Status switch
            {
                ClaimStatus.Pending => 25,
                ClaimStatus.Submitted => 50,
                ClaimStatus.Verified => 75,
                ClaimStatus.Approved => 100,
                _ => 0
            };

            return $@"
            <div class='progress mt-2'>
                <div class='progress-bar bg-success' style='width:{percentage}%'>
                    {status}
                </div>
            </div>";
        
        

           
        }

    }
}

