using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ContractMonthlyClaimSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        // DbSets for your application entities
        public DbSet<LecturerClaim> Claims { get; set; }

        public DbSet<ClaimVerification> ClaimVerifications { get; set; }
        public DbSet<LecturerClaim> LecturerClaims { get; set; }
        public DbSet<CoordinatorApproval> CoordinatorApprovals { get; set; }
        public DbSet<ManagerApproval> ManagerApprovals { get; set; }

        // USER PROFILE TABLE
        public DbSet<UserProfile> UserProfiles { get; set; }

        // Optional: Model configuration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Claim relationships
            builder.Entity<LecturerClaim>()
                .HasOne(c => c.CoordinatorApproval)
                .WithOne(a => a.Claim )
                .HasForeignKey<CoordinatorApproval>(a => a.LecturerClaimId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LecturerClaim>()
                .HasOne(c => c.ManagerApproval)
                .WithOne(a => a.LecturerClaim)
                .HasForeignKey<ManagerApproval>(a => a.LecturerClaimId)
                .OnDelete(DeleteBehavior.Restrict);

            // Identity table name customizations
            builder.Entity<ApplicationUser>(entity => entity.ToTable("Users"));
            builder.Entity<IdentityRole>(entity => entity.ToTable("Roles"));
            builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
            builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));

            // Claim entity custom configuration
            builder.Entity<LecturerClaim>(entity =>
            {
                entity.HasKey(c => c.ClaimId);
                entity.Property(c => c.LecturerName).HasMaxLength(150).IsRequired();
                entity.Property(c => c.Status).HasConversion<string>();
            });
        }
    }
}
