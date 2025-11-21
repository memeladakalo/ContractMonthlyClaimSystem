# Contract Monthly Claim System (CMCS)

## Features
- Lecturer claim submission form
- Claim tracking dashboard
- Coordinator claim verification
- Manager claim approval
- Responsive Bootstrap UI
- HR view and report generator
## Getting Started
1. Open `ContractMonthlyClaimSystem.csproj` in Visual Studio 2022+
2. Ensure .NET 8.0 SDK is installed
3. Run with `F5`

---
## Commit Messages
Commit: feat: add contract monthly claim form validation
-Implemented front-end back-end validation for rhe system form to ensure data integrity and prevent errors.
Commit: fix: resolve claim calculation bug for partial months
-fixed issue where claims for partial months were calculating incorrectly, ensuring accurate payouts for contractora.
Commit: docs: update UI for claim submission confirmation
-Improved user experience with a clearer confirmation message and summary after claim submission
Commit: feat: Integrate email notifications for claim status updates
-added automated email notifications to contactors on claim appproval or rejection
Commit: fix: Commit fix- handle claim submission errors gracefully
-impemented error handling for claim submissions, providing user-friendly feedback on failures
Commit perf: optimize database queries for claim retrieval
-Descriprtion: Optimized queries for faster retrieval of contract claim data, improving system perfomance.
Commit: Chore: update dependencies for security patches
-Description: Updated project dependencies to include latest security patches and fixes.
