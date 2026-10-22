using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscoverEgypt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CulturalAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CulturalAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProfileImageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuideProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HostProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    HostType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProfileImageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TouristPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Budget = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GuidePreference = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TouristPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuideCulturalAreas",
                columns: table => new
                {
                    GuideProfileId = table.Column<int>(type: "int", nullable: false),
                    CulturalAreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideCulturalAreas", x => new { x.GuideProfileId, x.CulturalAreaId });
                    table.ForeignKey(
                        name: "FK_GuideCulturalAreas_CulturalAreas_CulturalAreaId",
                        column: x => x.CulturalAreaId,
                        principalTable: "CulturalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuideCulturalAreas_GuideProfiles_GuideProfileId",
                        column: x => x.GuideProfileId,
                        principalTable: "GuideProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideLanguages",
                columns: table => new
                {
                    GuideProfileId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    IsNative = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideLanguages", x => new { x.GuideProfileId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_GuideLanguages_GuideProfiles_GuideProfileId",
                        column: x => x.GuideProfileId,
                        principalTable: "GuideProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuideProfileId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LicenseExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedByAdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRequests", x => x.Id);
                    table.CheckConstraint("CK_VerificationRequest_ReviewFields", "([ReviewedByAdminId] IS NULL AND [ReviewedAt] IS NULL) OR ([ReviewedByAdminId] IS NOT NULL AND [ReviewedAt] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_VerificationRequests_AspNetUsers_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VerificationRequests_GuideProfiles_GuideProfileId",
                        column: x => x.GuideProfileId,
                        principalTable: "GuideProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HostProfileId = table.Column<int>(type: "int", nullable: false),
                    ExperienceTypeId = table.Column<int>(type: "int", nullable: false),
                    CulturalAreaId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    CulturalStory = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    PricePerPerson = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxGroupSize = table.Column<int>(type: "int", nullable: false),
                    MeetingPointArea = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ExactMeetingPoint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedByAdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.Id);
                    table.CheckConstraint("CK_Experience_DurationMinutes_Positive", "[DurationMinutes] > 0");
                    table.CheckConstraint("CK_Experience_MaxGroupSize_Positive", "[MaxGroupSize] > 0");
                    table.CheckConstraint("CK_Experience_PricePerPerson_Positive", "[PricePerPerson] > 0");
                    table.ForeignKey(
                        name: "FK_Experiences_AspNetUsers_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experiences_CulturalAreas_CulturalAreaId",
                        column: x => x.CulturalAreaId,
                        principalTable: "CulturalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experiences_ExperienceTypes_ExperienceTypeId",
                        column: x => x.ExperienceTypeId,
                        principalTable: "ExperienceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experiences_HostProfiles_HostProfileId",
                        column: x => x.HostProfileId,
                        principalTable: "HostProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreferenceCulturalAreas",
                columns: table => new
                {
                    TouristPreferenceId = table.Column<int>(type: "int", nullable: false),
                    CulturalAreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenceCulturalAreas", x => new { x.TouristPreferenceId, x.CulturalAreaId });
                    table.ForeignKey(
                        name: "FK_PreferenceCulturalAreas_CulturalAreas_CulturalAreaId",
                        column: x => x.CulturalAreaId,
                        principalTable: "CulturalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreferenceCulturalAreas_TouristPreferences_TouristPreferenceId",
                        column: x => x.TouristPreferenceId,
                        principalTable: "TouristPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferenceExperienceTypes",
                columns: table => new
                {
                    TouristPreferenceId = table.Column<int>(type: "int", nullable: false),
                    ExperienceTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenceExperienceTypes", x => new { x.TouristPreferenceId, x.ExperienceTypeId });
                    table.ForeignKey(
                        name: "FK_PreferenceExperienceTypes_ExperienceTypes_ExperienceTypeId",
                        column: x => x.ExperienceTypeId,
                        principalTable: "ExperienceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreferenceExperienceTypes_TouristPreferences_TouristPreferenceId",
                        column: x => x.TouristPreferenceId,
                        principalTable: "TouristPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferenceInterests",
                columns: table => new
                {
                    TouristPreferenceId = table.Column<int>(type: "int", nullable: false),
                    InterestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenceInterests", x => new { x.TouristPreferenceId, x.InterestId });
                    table.ForeignKey(
                        name: "FK_PreferenceInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreferenceInterests_TouristPreferences_TouristPreferenceId",
                        column: x => x.TouristPreferenceId,
                        principalTable: "TouristPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferenceLanguages",
                columns: table => new
                {
                    TouristPreferenceId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenceLanguages", x => new { x.TouristPreferenceId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_PreferenceLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreferenceLanguages_TouristPreferences_TouristPreferenceId",
                        column: x => x.TouristPreferenceId,
                        principalTable: "TouristPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VerificationRequestId = table.Column<int>(type: "int", nullable: false),
                    DocType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    CulturalAreaId = table.Column<int>(type: "int", nullable: true),
                    StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDocuments", x => x.Id);
                    table.CheckConstraint("CK_VerificationDocument_TypeReferences", "([DocType] = 'Language' AND [LanguageId] IS NOT NULL AND [CulturalAreaId] IS NULL) OR ([DocType] = 'Residence' AND [CulturalAreaId] IS NOT NULL AND [LanguageId] IS NULL) OR ([DocType] NOT IN ('Language', 'Residence') AND [LanguageId] IS NULL AND [CulturalAreaId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_CulturalAreas_CulturalAreaId",
                        column: x => x.CulturalAreaId,
                        principalTable: "CulturalAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_VerificationRequests_VerificationRequestId",
                        column: x => x.VerificationRequestId,
                        principalTable: "VerificationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TouristUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ProviderUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BookingType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExperienceId = table.Column<int>(type: "int", nullable: true),
                    GuideProfileId = table.Column<int>(type: "int", nullable: true),
                    ScheduledStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: true),
                    GuideHours = table.Column<int>(type: "int", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PricePerPersonSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    HourlyRateSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ServiceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformFeeRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    PlatformFeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.CheckConstraint("CK_Booking_PlatformFeeRate_Range", "[PlatformFeeRate] >= 0 AND [PlatformFeeRate] <= 1");
                    table.CheckConstraint("CK_Booking_ScheduledPeriod", "[ScheduledEnd] > [ScheduledStart]");
                    table.CheckConstraint("CK_Booking_ServiceAmount_NonNegative", "[ServiceAmount] >= 0");
                    table.CheckConstraint("CK_Booking_TotalAmount_NonNegative", "[TotalAmount] >= 0");
                    table.CheckConstraint("CK_Booking_ValidTypeTarget", "([BookingType] = 'Experience' AND [ExperienceId] IS NOT NULL AND [GuideProfileId] IS NULL AND [NumberOfGuests] > 0 AND [GuideHours] IS NULL AND [Destination] IS NULL AND [PricePerPersonSnapshot] IS NOT NULL AND [HourlyRateSnapshot] IS NULL) OR ([BookingType] = 'Guide' AND [GuideProfileId] IS NOT NULL AND [ExperienceId] IS NULL AND [NumberOfGuests] IS NULL AND [GuideHours] > 0 AND [PricePerPersonSnapshot] IS NULL AND [HourlyRateSnapshot] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_ProviderUserId",
                        column: x => x.ProviderUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_AspNetUsers_TouristUserId",
                        column: x => x.TouristUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_GuideProfiles_GuideProfileId",
                        column: x => x.GuideProfileId,
                        principalTable: "GuideProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceDeclarations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperienceId = table.Column<int>(type: "int", nullable: false),
                    HostUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DeclarationType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DeclarationText = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienceDeclarations_AspNetUsers_HostUserId",
                        column: x => x.HostUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExperienceDeclarations_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceEvidence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperienceId = table.Column<int>(type: "int", nullable: false),
                    EvidenceKind = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TextContent = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceEvidence", x => x.Id);
                    table.CheckConstraint("CK_ExperienceEvidence_HasContent", "[TextContent] IS NOT NULL OR [StorageKey] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_ExperienceEvidence_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceInterests",
                columns: table => new
                {
                    ExperienceId = table.Column<int>(type: "int", nullable: false),
                    InterestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceInterests", x => new { x.ExperienceId, x.InterestId });
                    table.ForeignKey(
                        name: "FK_ExperienceInterests_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperienceInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceLanguages",
                columns: table => new
                {
                    ExperienceId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceLanguages", x => new { x.ExperienceId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_ExperienceLanguages_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperienceLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperienceId = table.Column<int>(type: "int", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsCover = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceMedia", x => x.Id);
                    table.CheckConstraint("CK_ExperienceMedia_CoverMustBeImage", "[IsCover] = 0 OR [MediaType] = 'Image'");
                    table.CheckConstraint("CK_ExperienceMedia_DisplayOrder_NonNegative", "[DisplayOrder] >= 0");
                    table.ForeignKey(
                        name: "FK_ExperienceMedia_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingShares_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Disputes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    OpenedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ResolutionNote = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    ResolvedByAdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disputes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disputes_AspNetUsers_OpenedByUserId",
                        column: x => x.OpenedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Disputes_AspNetUsers_ResolvedByAdminId",
                        column: x => x.ResolvedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Disputes_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.CheckConstraint("CK_Payment_Amount_NonNegative", "[Amount] >= 0");
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EligibleAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payouts", x => x.Id);
                    table.CheckConstraint("CK_Payout_Amount_NonNegative", "[Amount] >= 0");
                    table.ForeignKey(
                        name: "FK_Payouts_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Review_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    DisputeId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DecidedByAdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.CheckConstraint("CK_Refund_Amount_Positive", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_Refunds_AspNetUsers_DecidedByAdminId",
                        column: x => x.DecidedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Refunds_Disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalTable: "Disputes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sanctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DisputeId = table.Column<int>(type: "int", nullable: false),
                    SanctionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedByAdminId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sanctions", x => x.Id);
                    table.CheckConstraint("CK_Sanction_Days_Positive", "[Days] > 0");
                    table.CheckConstraint("CK_Sanction_ValidPeriod", "[EndsAt] > [StartsAt]");
                    table.ForeignKey(
                        name: "FK_Sanctions_AspNetUsers_DecidedByAdminId",
                        column: x => x.DecidedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sanctions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sanctions_Disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalTable: "Disputes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ExperienceId",
                table: "Bookings",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GuideProfileId",
                table: "Bookings",
                column: "GuideProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ProviderUserId",
                table: "Bookings",
                column: "ProviderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TouristUserId",
                table: "Bookings",
                column: "TouristUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingShares_BookingId",
                table: "BookingShares",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingShares_Token",
                table: "BookingShares",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CulturalAreas_Name",
                table: "CulturalAreas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_BookingId",
                table: "Disputes",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_OpenedByUserId",
                table: "Disputes",
                column: "OpenedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_ResolvedByAdminId",
                table: "Disputes",
                column: "ResolvedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceDeclarations_ExperienceId",
                table: "ExperienceDeclarations",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceDeclarations_HostUserId",
                table: "ExperienceDeclarations",
                column: "HostUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceEvidence_ExperienceId",
                table: "ExperienceEvidence",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceInterests_InterestId",
                table: "ExperienceInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceLanguages_LanguageId",
                table: "ExperienceLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceMedia_ExperienceId_DisplayOrder",
                table: "ExperienceMedia",
                columns: new[] { "ExperienceId", "DisplayOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_CulturalAreaId",
                table: "Experiences",
                column: "CulturalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_ExperienceTypeId",
                table: "Experiences",
                column: "ExperienceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_HostProfileId",
                table: "Experiences",
                column: "HostProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_ReviewedByAdminId",
                table: "Experiences",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceTypes_Name",
                table: "ExperienceTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuideCulturalAreas_CulturalAreaId",
                table: "GuideCulturalAreas",
                column: "CulturalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideLanguages_LanguageId",
                table: "GuideLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideProfiles_UserId",
                table: "GuideProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostProfiles_UserId",
                table: "HostProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interests_Name",
                table: "Interests",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Name",
                table: "Languages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BookingId",
                table: "Notifications",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_BookingId",
                table: "Payouts",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreferenceCulturalAreas_CulturalAreaId",
                table: "PreferenceCulturalAreas",
                column: "CulturalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenceExperienceTypes_ExperienceTypeId",
                table: "PreferenceExperienceTypes",
                column: "ExperienceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenceInterests_InterestId",
                table: "PreferenceInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenceLanguages_LanguageId",
                table: "PreferenceLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_BookingId",
                table: "Refunds",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_DecidedByAdminId",
                table: "Refunds",
                column: "DecidedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_DisputeId",
                table: "Refunds",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                table: "Reviews",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sanctions_DecidedByAdminId",
                table: "Sanctions",
                column: "DecidedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Sanctions_DisputeId",
                table: "Sanctions",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sanctions_UserId",
                table: "Sanctions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TouristPreferences_UserId",
                table: "TouristPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_CulturalAreaId",
                table: "VerificationDocuments",
                column: "CulturalAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_LanguageId",
                table: "VerificationDocuments",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_VerificationRequestId",
                table: "VerificationDocuments",
                column: "VerificationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_GuideProfileId",
                table: "VerificationRequests",
                column: "GuideProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_ReviewedByAdminId",
                table: "VerificationRequests",
                column: "ReviewedByAdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookingShares");

            migrationBuilder.DropTable(
                name: "ExperienceDeclarations");

            migrationBuilder.DropTable(
                name: "ExperienceEvidence");

            migrationBuilder.DropTable(
                name: "ExperienceInterests");

            migrationBuilder.DropTable(
                name: "ExperienceLanguages");

            migrationBuilder.DropTable(
                name: "ExperienceMedia");

            migrationBuilder.DropTable(
                name: "GuideCulturalAreas");

            migrationBuilder.DropTable(
                name: "GuideLanguages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Payouts");

            migrationBuilder.DropTable(
                name: "PreferenceCulturalAreas");

            migrationBuilder.DropTable(
                name: "PreferenceExperienceTypes");

            migrationBuilder.DropTable(
                name: "PreferenceInterests");

            migrationBuilder.DropTable(
                name: "PreferenceLanguages");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Sanctions");

            migrationBuilder.DropTable(
                name: "VerificationDocuments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "TouristPreferences");

            migrationBuilder.DropTable(
                name: "Disputes");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "VerificationRequests");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.DropTable(
                name: "GuideProfiles");

            migrationBuilder.DropTable(
                name: "CulturalAreas");

            migrationBuilder.DropTable(
                name: "ExperienceTypes");

            migrationBuilder.DropTable(
                name: "HostProfiles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
