namespace LetterGenerator.Core.Models;

/// <summary>
/// Model for Authorization Denial / Adverse Determination letters.
/// Includes denial reasons, clinical rationale, and appeal rights
/// as required by CMS and state regulations.
/// </summary>
public class DenialLetterModel : LetterBase
{
    public DenialLetterModel()
    {
        LetterType = "DENIAL";
    }

    // ── Denial-Specific Fields ───────────────────────────────────────
    public string DenialType { get; set; } = "Medical Necessity";  // Medical Necessity, Not a Covered Benefit, Out of Network, etc.
    public string DenialCategory { get; set; } = "Initial";        // Initial, Concurrent, Retrospective
    public string UrgencyLevel { get; set; } = "Standard";         // Urgent, Standard, Expedited

    // ── Clinical Rationale ───────────────────────────────────────────
    public string ClinicalRationale { get; set; } = string.Empty;
    public string? GuidelineReference { get; set; }    // e.g., "InterQual 2024 – Inpatient Criteria"
    public string? ReviewerName { get; set; }
    public string? ReviewerCredentials { get; set; }   // e.g., "MD, Board Certified Internal Medicine"
    public DateTime? ReviewDate { get; set; }

    // ── Appeal Rights (regulatory requirement) ───────────────────────
    public AppealRightsInfo AppealRights { get; set; } = new();

    // ── Additional Denial Info ───────────────────────────────────────
    public List<string> AdditionalNotes { get; set; } = [];
    public string? ExternalReviewInfo { get; set; }
    public string? MedicaidFairHearingInfo { get; set; }  // Required for Medicaid denials
}

public class AppealRightsInfo
{
    public int AppealDeadlineDays { get; set; } = 180;
    public DateTime? AppealDeadlineDate { get; set; }
    public string AppealSubmissionAddress { get; set; } = string.Empty;
    public string? AppealPhone { get; set; }
    public string? AppealFax { get; set; }
    public bool HasExpeditedAppealRight { get; set; } = true;
    public int ExpeditedAppealDeadlineDays { get; set; } = 72;
    public string? ExternalReviewOrganization { get; set; }
    public string? ExternalReviewPhone { get; set; }
    public string? StateInsuranceDepartmentInfo { get; set; }
}
