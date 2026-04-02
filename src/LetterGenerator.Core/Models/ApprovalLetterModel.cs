namespace LetterGenerator.Core.Models;

/// <summary>
/// Model for Authorization Approval letters.
/// </summary>
public class ApprovalLetterModel : LetterBase
{
    public ApprovalLetterModel()
    {
        LetterType = "APPROVAL";
    }

    // ── Approval-Specific Fields ─────────────────────────────────────
    public string ApprovalType { get; set; } = "Full";  // Full, Partial, Modified
    public int? ApprovedUnits { get; set; }
    public int? ApprovedDays { get; set; }
    public DateTime? ApprovalExpirationDate { get; set; }

    // ── Conditions / Limitations ─────────────────────────────────────
    public List<string> Conditions { get; set; } = [];          // e.g., "Must be performed at an in-network facility"
    public List<string> Limitations { get; set; } = [];         // e.g., "Maximum of 12 visits per benefit year"

    // ── Partial Approval Specifics ───────────────────────────────────
    public string? PartialApprovalExplanation { get; set; }
    public List<ServiceLineItem> DeniedServiceLines { get; set; } = [];

    // ── Additional ──────────────────────────────────────────────────
    public string? ReviewerName { get; set; }
    public List<string> AdditionalNotes { get; set; } = [];
}
