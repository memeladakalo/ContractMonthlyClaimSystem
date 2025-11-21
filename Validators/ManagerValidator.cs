using ContractMonthlyClaimSystem.Models;
using FluentValidation;

namespace ContractMonthlyClaimSystem.Validators
{
    public class ManagerValidator : AbstractValidator<LecturerClaim>
    {
        public ManagerValidator()
        {
            RuleFor(c => c)
                .Must(c => c.Status == ClaimStatus.Verified)
                .WithMessage("Only VERIFIED claims can be approved by Manager.");

            RuleFor(c => c)
                .Must(c => c.Status != ClaimStatus.Approved)
                .WithMessage("Approved claims cannot be rejected by Manager.");
        }
    }
}
