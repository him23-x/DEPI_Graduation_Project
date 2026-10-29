using System.Linq.Expressions;
using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums.Experiences;
using DiscoverEgypt.Domain.Enums.Hosts;
using DiscoverEgypt.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiscoverEgypt.Persistence.Seeding;

public static class DevelopmentDataSeeder
{
    private static readonly string[] ExperienceTypeNames =
    [
        "Food & Cooking",
        "Arts & Crafts",
        "Local Traditions & Daily Life"
    ];

    private static readonly string[] CulturalAreaNames =
    [
        "Cairo",
        "Giza",
        "Alexandria",
        "Luxor",
        "Aswan",
        "Red Sea",
        "Other"
    ];

    private static readonly string[] InterestNames =
    [
        "Egyptian Food",
        "Local Culture",
        "History",
        "Arts & Crafts",
        "Traditional Markets",
        "Local Communities",
        "Nature",
        "Family Experiences",
        "Workshops",
        "Egyptian Traditions"
    ];

    private static readonly string[] LanguageNames =
    [
        "English",
        "Arabic",
        "French",
        "German",
        "Spanish"
    ];

    public static async Task SeedAsync(IServiceProvider services, string demoPassword)
    {
        if (string.IsNullOrWhiteSpace(demoPassword))
            throw new InvalidOperationException("DevelopmentSeed:DemoPassword must be configured to create demo accounts.");

        await using var scope = services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<DiscoverEgyptDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedLookupsAsync(dbContext);

        var hostUser = await GetOrCreateUserAsync(userManager, "demo.host@discoveregypt.test", "Demo Host", IdentityRoles.Host, demoPassword);
        var touristUser = await GetOrCreateUserAsync(userManager, "demo.tourist@discoveregypt.test", "Demo Tourist", IdentityRoles.Tourist, demoPassword);
        var guideUser = await GetOrCreateUserAsync(userManager, "demo.guide@discoveregypt.test", "Demo Guide", IdentityRoles.Guide, demoPassword);
        await GetOrCreateUserAsync(userManager, "demo.admin@discoveregypt.test", "Demo Admin", IdentityRoles.Admin, demoPassword);

        var hostProfile = await dbContext.HostProfiles.SingleOrDefaultAsync(profile => profile.UserId == hostUser.Id);
        if (hostProfile is null)
        {
            hostProfile = new HostProfile
            {
                UserId = hostUser.Id,
                HostType = HostType.Family,
                Bio = "A Cairo family sharing Egyptian food, traditions, and everyday stories with visitors.",
                CreatedAt = DateTime.UtcNow
            };
            dbContext.HostProfiles.Add(hostProfile);
            await dbContext.SaveChangesAsync();
        }

        var guideProfile = await dbContext.GuideProfiles.SingleOrDefaultAsync(profile => profile.UserId == guideUser.Id);
        if (guideProfile is null)
        {
            guideProfile = new GuideProfile
            {
                UserId = guideUser.Id,
                Bio = "A local guide sharing the history and culture of Cairo.",
                HourlyRate = 350m,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.GuideProfiles.Add(guideProfile);
            await dbContext.SaveChangesAsync();
        }

        await EnsureGuideLookupsAsync(dbContext, guideProfile.Id);
        await SeedDemoExperiencesAsync(dbContext, hostProfile.Id);
    }

    private static async Task SeedLookupsAsync(DiscoverEgyptDbContext dbContext)
    {
        await AddMissingAsync(dbContext.ExperienceTypes, ExperienceTypeNames, item => item.Name,
            name => new ExperienceType { Name = name });
        await AddMissingAsync(dbContext.CulturalAreas, CulturalAreaNames, item => item.Name,
            name => new CulturalArea { Name = name });
        await AddMissingAsync(dbContext.Interests, InterestNames, item => item.Name,
            name => new Interest { Name = name });
        await AddMissingAsync(dbContext.Languages, LanguageNames, item => item.Name,
            name => new Language { Name = name });

        await dbContext.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> GetOrCreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string role,
        string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not create development account '{email}': {errors}");
            }
        }

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            if (roles.Count != 0)
                throw new InvalidOperationException($"Development account '{email}' already has a different role.");

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Could not assign the '{role}' role to '{email}': {errors}");
            }
        }

        return user;
    }

    private static async Task EnsureGuideLookupsAsync(DiscoverEgyptDbContext dbContext, int guideProfileId)
    {
        var englishId = await dbContext.Languages.Where(language => language.Name == "English")
            .Select(language => language.Id).SingleAsync();
        var cairoId = await dbContext.CulturalAreas.Where(area => area.Name == "Cairo")
            .Select(area => area.Id).SingleAsync();

        if (!await dbContext.GuideLanguages.AnyAsync(item => item.GuideProfileId == guideProfileId && item.LanguageId == englishId))
            dbContext.GuideLanguages.Add(new GuideLanguage { GuideProfileId = guideProfileId, LanguageId = englishId, IsNative = false });
        if (!await dbContext.GuideCulturalAreas.AnyAsync(item => item.GuideProfileId == guideProfileId && item.CulturalAreaId == cairoId))
            dbContext.GuideCulturalAreas.Add(new GuideCulturalArea { GuideProfileId = guideProfileId, CulturalAreaId = cairoId });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDemoExperiencesAsync(DiscoverEgyptDbContext dbContext, int hostProfileId)
    {
        var experienceTypeIds = await dbContext.ExperienceTypes.ToDictionaryAsync(item => item.Name, item => item.Id);
        var culturalAreaIds = await dbContext.CulturalAreas.ToDictionaryAsync(item => item.Name, item => item.Id);
        var interestIds = await dbContext.Interests.ToDictionaryAsync(item => item.Name, item => item.Id);
        var languageIds = await dbContext.Languages.ToDictionaryAsync(item => item.Name, item => item.Id);

        await AddDemoExperienceAsync(
            dbContext, hostProfileId, "[DEMO] Family Cooking in Historic Cairo",
            "Prepare a traditional Egyptian meal with a local family and learn the stories behind each dish.",
            "Recipes and cooking traditions passed down through generations in our family kitchen.",
            experienceTypeIds["Food & Cooking"], culturalAreaIds["Cairo"],
            [interestIds["Egyptian Food"], interestIds["Family Experiences"]],
            [languageIds["Arabic"], languageIds["English"]],
            ExperienceStatus.Draft, null);

        await AddDemoExperienceAsync(
            dbContext, hostProfileId, "[DEMO] Handmade Traditions of Cairo",
            "Meet local makers and learn about the techniques used in traditional Egyptian crafts.",
            "A hands-on introduction to craft traditions that continue to shape daily life in Cairo.",
            experienceTypeIds["Arts & Crafts"], culturalAreaIds["Cairo"],
            [interestIds["Arts & Crafts"], interestIds["Local Communities"]],
            [languageIds["Arabic"], languageIds["English"]],
            ExperienceStatus.Rejected, "Please add clearer details about the workshop activities before resubmitting.");
    }

    private static async Task AddDemoExperienceAsync(
        DiscoverEgyptDbContext dbContext,
        int hostProfileId,
        string title,
        string description,
        string culturalStory,
        int experienceTypeId,
        int culturalAreaId,
        int[] interestIds,
        int[] languageIds,
        ExperienceStatus status,
        string? rejectionReason)
    {
        if (await dbContext.Experiences.AnyAsync(item => item.HostProfileId == hostProfileId && item.Title == title))
            return;

        var now = DateTime.UtcNow;
        var experience = new Experience
        {
            HostProfileId = hostProfileId,
            ExperienceTypeId = experienceTypeId,
            CulturalAreaId = culturalAreaId,
            Title = title,
            Description = description,
            CulturalStory = culturalStory,
            PricePerPerson = 650m,
            DurationMinutes = 180,
            MaxGroupSize = 6,
            MeetingPointArea = "Historic Cairo",
            ExactMeetingPoint = "A sample private meeting address for development testing.",
            Status = status,
            RejectionReason = rejectionReason,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Experiences.Add(experience);
        await dbContext.SaveChangesAsync();

        dbContext.ExperienceInterests.AddRange(interestIds.Select(interestId => new ExperienceInterest
        {
            ExperienceId = experience.Id,
            InterestId = interestId
        }));
        dbContext.ExperienceLanguages.AddRange(languageIds.Select(languageId => new ExperienceLanguage
        {
            ExperienceId = experience.Id,
            LanguageId = languageId
        }));
        await dbContext.SaveChangesAsync();
    }

    private static async Task AddMissingAsync<T>(
        DbSet<T> set,
        IEnumerable<string> expectedNames,
        Expression<Func<T, string>> nameSelector,
        Func<string, T> createEntity)
        where T : class
    {
        var existingNames = (await set.AsNoTracking().Select(nameSelector).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingEntities = expectedNames
            .Where(name => !existingNames.Contains(name))
            .Select(createEntity)
            .ToArray();

        if (missingEntities.Length > 0)
            await set.AddRangeAsync(missingEntities);
    }
}
