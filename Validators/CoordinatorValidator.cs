using ContractMonthlyClaimSystem.Models;
using FluentValidation;

namespace ContractMonthlyClaimSystem.Validators
{
    public class CoordinatorValidator : AbstractValidator<LecturerClaim>
    {
        public CoordinatorValidator()
        {
            RuleFor(c => c)
                .Must(c => c.Status == ClaimStatus.Submitted)
                .WithMessage("Only claims with status 'Submitted' can be verified by Coordinator.");

            RuleFor(c => c)
                .Must(c => c.Status != ClaimStatus.Approved)
                .When(c => c.Status == ClaimStatus.Rejected)
                .WithMessage("Approved claims cannot be rejected by Coordinator.");
        }
    }
}
