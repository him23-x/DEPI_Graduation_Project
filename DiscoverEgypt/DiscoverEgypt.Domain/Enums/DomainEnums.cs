namespace DiscoverEgypt.Domain.Enums;

public enum HostType { Family, Individual, Artisan, Farmer }
public enum VerificationStatus { Pending, Approved, Rejected, Expired }
public enum VerificationDocType { NationalId, License, Language, Residence, Permission }
public enum Budget { Under500, From500To800, From800To1500, From1500To3000, Over3000 }
public enum DurationPreference { Under2h, From2To4h, From4To6h, FullDay }
public enum GuidePreference { Preferred, NoGuide, Either }
public enum ExperienceStatus { Draft, PendingApproval, Published, Rejected, Unpublished }
public enum MediaType { Image, Video }
public enum EvidenceKind { MenuAndAllergenInfo, FoodAreaPhoto, PortfolioPhoto, WorkshopPhoto, RouteDetails, VenueProof, VenuePermission }
public enum DeclarationType { FoodSafety, ToolsMaterialsSafety, PrivacyVenuePermission }
public enum BookingType { Experience, Guide }
public enum BookingStatus { Pending, Confirmed, Rejected, Cancelled, Completed }
public enum PaymentStatus { Pending, Paid, Failed, Refunded, PartiallyRefunded }
public enum PayoutStatus { Pending, Paid, Cancelled }
public enum DisputeStatus { Open, UnderReview, Resolved, Closed }
public enum DisputeOutcome { ProviderFault, TouristFault, NoFault }
public enum SanctionType { ProviderSuspension, TouristBan }
public enum NotificationType { BookingConfirmation, BookingReminder, PaymentUpdate, DisputeUpdate, GuideSuggestion, NewBooking, BookingStatusUpdate, ExperienceApprovalUpdate, VerificationUpdate }
