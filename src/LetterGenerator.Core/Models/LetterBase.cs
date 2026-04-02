namespace LetterGenerator.Core.Models;

/// <summary>
/// Base model containing fields common to all healthcare letters.
/// </summary>
public abstract class LetterBase
{
    // ── Letter Metadata ──────────────────────────────────────────────
    public string LetterType { get; set; } = string.Empty;
    public string LetterReferenceId { get; set; } = Guid.NewGuid().ToString("N")[..12].ToUpper();
    public DateTime LetterDate { get; set; } = DateTime.Today;
    public DateTime? EffectiveDate { get; set; }

    // ── Organization / Plan ──────────────────────────────────────────
    public OrganizationInfo Organization { get; set; } = new();
    public PlanInfo Plan { get; set; } = new();

    // ── Member Info ──────────────────────────────────────────────────
    public MemberInfo Member { get; set; } = new();

    // ── Provider Info ────────────────────────────────────────────────
    public ProviderInfo RequestingProvider { get; set; } = new();
    public ProviderInfo? ServicingProvider { get; set; }

    // ── Authorization Details ────────────────────────────────────────
    public AuthorizationInfo Authorization { get; set; } = new();

    // ── Contact / Footer ─────────────────────────────────────────────
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactFax { get; set; } = string.Empty;
    public string ContactHours { get; set; } = "Monday – Friday, 8:00 AM – 5:00 PM CT";
    public string? Website { get; set; }
    public string? AdditionalFooterText { get; set; }

    // ── Logos / Images (base64 or file path) ─────────────────────────
    public string? LogoBase64 { get; set; }
    public string LogoContentType { get; set; } = "image/png";
}

public class OrganizationInfo
{
    public string Name { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Fax { get; set; }

    public string FullAddress => string.IsNullOrWhiteSpace(AddressLine2)
        ? $"{AddressLine1}, {City}, {State} {ZipCode}"
        : $"{AddressLine1}, {AddressLine2}, {City}, {State} {ZipCode}";
}

public class PlanInfo
{
    public string PlanName { get; set; } = string.Empty;
    public string? PlanId { get; set; }
    public string? GroupNumber { get; set; }
    public string LineOfBusiness { get; set; } = string.Empty; // Commercial, Medicare, Medicaid
}

public class MemberInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string FullAddress => string.IsNullOrWhiteSpace(AddressLine2)
        ? $"{AddressLine1}, {City}, {State} {ZipCode}"
        : $"{AddressLine1}, {AddressLine2}, {City}, {State} {ZipCode}";
}

public class ProviderInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Npi { get; set; }
    public string? TaxId { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}

public class AuthorizationInfo
{
    public string AuthorizationNumber { get; set; } = string.Empty;
    public DateTime? RequestDate { get; set; }
    public DateTime? DecisionDate { get; set; }
    public DateTime? ServiceStartDate { get; set; }
    public DateTime? ServiceEndDate { get; set; }
    public string? ServiceType { get; set; }       // Inpatient, Outpatient, DME, etc.
    public string? PlaceOfService { get; set; }
    public List<ServiceLineItem> ServiceLines { get; set; } = [];
}

public class ServiceLineItem
{
    public string ProcedureCode { get; set; } = string.Empty; // CPT / HCPCS
    public string Description { get; set; } = string.Empty;
    public int? UnitsRequested { get; set; }
    public int? UnitsApproved { get; set; }
    public string? Modifier { get; set; }
    public string? Status { get; set; } // Approved, Denied, Partially Approved
    public string? DenialReasonCode { get; set; }
    public string? DenialReasonDescription { get; set; }
}
