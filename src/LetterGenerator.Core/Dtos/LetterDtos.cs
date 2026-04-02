using LetterGenerator.Core.Models;

namespace LetterGenerator.Core.Dtos;

// ── Request DTOs ─────────────────────────────────────────────────────

public class GenerateLetterRequest<TModel> where TModel : LetterBase
{
    public required TModel LetterData { get; set; }
    public PdfOutputOptions? PdfOptions { get; set; }
    public string OutputFormat { get; set; } = "pdf";  // "pdf" or "html"
}

public class PdfOutputOptions
{
    public string PageSize { get; set; } = "Letter";
    public bool Landscape { get; set; } = false;
}

public class DenialLetterRequest : GenerateLetterRequest<DenialLetterModel> { }
public class ApprovalLetterRequest : GenerateLetterRequest<ApprovalLetterModel> { }

// ── Response DTOs ────────────────────────────────────────────────────

public class LetterResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string LetterReferenceId { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public byte[]? FileBytes { get; set; }
    public string? HtmlContent { get; set; }
}

// ── Batch DTOs ───────────────────────────────────────────────────────

public class BatchLetterRequest
{
    public string LetterType { get; set; } = string.Empty;  // "DENIAL", "APPROVAL"
    public List<object> LetterDataList { get; set; } = [];
}

public class BatchLetterResponse
{
    public int TotalRequested { get; set; }
    public int Succeeded { get; set; }
    public int Failed { get; set; }
    public List<BatchItemResult> Results { get; set; } = [];
}

public class BatchItemResult
{
    public int Index { get; set; }
    public bool Success { get; set; }
    public string? LetterReferenceId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FilePath { get; set; }
}
