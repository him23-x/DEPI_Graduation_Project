using System.ComponentModel.DataAnnotations;
using DiscoverEgypt.Application.DTOs.HostExperiences;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Presentation.Areas.Host.Models;

public sealed class CreateExperienceViewModel
{
    public int ExperienceId { get; set; }
    public ExperienceStatus? Status { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsEditing => ExperienceId > 0;

    [Range(1, int.MaxValue, ErrorMessage = "Choose an experience type.")]
    public int ExperienceTypeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose a cultural area.")]
    public int CulturalAreaId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(5000, ErrorMessage = "The cultural story must be 5,000 characters or fewer.")]
    [Display(Name = "Cultural story")]
    public string CulturalStory { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Enter a price greater than zero.")]
    [Display(Name = "Price per person (EGP)")]
    public decimal PricePerPerson { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than zero.")]
    [Display(Name = "Duration (minutes)")]
    public int DurationMinutes { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Group size must be at least one.")]
    [Display(Name = "Maximum group size")]
    public int MaxGroupSize { get; set; }

    [Required, StringLength(300)]
    [Display(Name = "Meeting point area")]
    public string MeetingPointArea { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    [Display(Name = "Exact meeting point")]
    public string ExactMeetingPoint { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "Choose at least one interest.")]
    public int[] InterestIds { get; set; } = [];

    [MinLength(1, ErrorMessage = "Choose at least one language.")]
    public int[] LanguageIds { get; set; } = [];

    public HostExperienceCreateOptions Options { get; set; } = new([], [], [], []);

    public bool HasRequiredLookups => Options.ExperienceTypes.Count > 0
        && Options.CulturalAreas.Count > 0
        && Options.Interests.Count > 0
        && Options.Languages.Count > 0;
}
