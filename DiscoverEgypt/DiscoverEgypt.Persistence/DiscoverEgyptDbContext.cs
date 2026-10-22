using DiscoverEgypt.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Persistence;

public class DiscoverEgyptDbContext : IdentityDbContext<ApplicationUser>
{
    public DiscoverEgyptDbContext(DbContextOptions<DiscoverEgyptDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingShare> BookingShares => Set<BookingShare>();
    public DbSet<CulturalArea> CulturalAreas => Set<CulturalArea>();
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<ExperienceDeclaration> ExperienceDeclarations => Set<ExperienceDeclaration>();
    public DbSet<ExperienceEvidence> ExperienceEvidence => Set<ExperienceEvidence>();
    public DbSet<ExperienceInterest> ExperienceInterests => Set<ExperienceInterest>();
    public DbSet<ExperienceLanguage> ExperienceLanguages => Set<ExperienceLanguage>();
    public DbSet<ExperienceMedia> ExperienceMedia => Set<ExperienceMedia>();
    public DbSet<ExperienceType> ExperienceTypes => Set<ExperienceType>();
    public DbSet<GuideCulturalArea> GuideCulturalAreas => Set<GuideCulturalArea>();
    public DbSet<GuideLanguage> GuideLanguages => Set<GuideLanguage>();
    public DbSet<GuideProfile> GuideProfiles => Set<GuideProfile>();
    public DbSet<HostProfile> HostProfiles => Set<HostProfile>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<PreferenceCulturalArea> PreferenceCulturalAreas => Set<PreferenceCulturalArea>();
    public DbSet<PreferenceExperienceType> PreferenceExperienceTypes => Set<PreferenceExperienceType>();
    public DbSet<PreferenceInterest> PreferenceInterests => Set<PreferenceInterest>();
    public DbSet<PreferenceLanguage> PreferenceLanguages => Set<PreferenceLanguage>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Sanction> Sanctions => Set<Sanction>();
    public DbSet<TouristPreference> TouristPreferences => Set<TouristPreference>();
    public DbSet<VerificationDocument> VerificationDocuments => Set<VerificationDocument>();
    public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscoverEgyptDbContext).Assembly);
    }
}
