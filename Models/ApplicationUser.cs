using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [StringLength(50)]
        public string Role { get; set; } // Optional: Can mirror Identity Roles

        [Phone]
        public override string PhoneNumber { get; set; } // Inherited, can override validation if needed

        [StringLength(200)]
        public string Address { get; set; }

        // Additional properties specific to your claim system
        public DateTime DateJoined { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // You can add a profile picture path if needed
        public string ProfilePicturePath { get; set; }
    }
}
