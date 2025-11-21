using System;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class ErrorViewModel
    {
        [Display(Name = "Request ID")]
        public string? RequestId { get; set; }

        [Display(Name = "Error Message")]
        public string? ErrorMessage { get; set; }

        [Display(Name = "Error Details")]
        public string? ErrorDetails { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime ErrorTime { get; set; } = DateTime.UtcNow;

        [Display(Name = "Show Request ID")]
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
