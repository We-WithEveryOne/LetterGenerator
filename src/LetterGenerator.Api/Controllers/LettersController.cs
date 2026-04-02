using LetterGenerator.Core.Interfaces;
using LetterGenerator.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LetterGenerator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LettersController : ControllerBase
{
    private readonly ILetterService _letterService;
    private readonly ILogger<LettersController> _logger;

    public LettersController(ILetterService letterService, ILogger<LettersController> logger)
    {
        _letterService = letterService;
        _logger = logger;
    }

    // ═══════════════════════════════════════════════════════════════════
    //  DENIAL LETTER
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Generate a Denial Letter as PDF.
    /// </summary>
    [HttpPost("denial/pdf")]
    [Produces("application/pdf")]
    public async Task<IActionResult> GenerateDenialPdf([FromBody] DenialLetterModel model)
    {
        _logger.LogInformation("Generating Denial PDF for member: {MemberId}", model.Member.MemberId);

        var pdfBytes = await _letterService.GenerateLetterPdfAsync("DenialLetter", model);

        return File(pdfBytes, "application/pdf",
            $"Denial_{model.LetterReferenceId}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Preview a Denial Letter as HTML (for development/QA).
    /// </summary>
    [HttpPost("denial/preview")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewDenialHtml([FromBody] DenialLetterModel model)
    {
        var html = await _letterService.GenerateLetterHtmlAsync("DenialLetter", model);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Generate a Denial Letter with sample/test data (for quick testing).
    /// </summary>
    [HttpGet("denial/sample")]
    [Produces("application/pdf")]
    public async Task<IActionResult> GenerateSampleDenialPdf()
    {
        var model = SampleDataFactory.CreateSampleDenialLetter();
        var pdfBytes = await _letterService.GenerateLetterPdfAsync("DenialLetter", model);
        return File(pdfBytes, "application/pdf", $"SampleDenial_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Preview a Denial Letter with sample data as HTML.
    /// </summary>
    [HttpGet("denial/sample/preview")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewSampleDenialHtml()
    {
        var model = SampleDataFactory.CreateSampleDenialLetter();
        var html = await _letterService.GenerateLetterHtmlAsync("DenialLetter", model);
        return Content(html, "text/html");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  APPROVAL LETTER
    // ═══════════════════════════════════════════════════════════════════

    [HttpPost("approval/pdf")]
    [Produces("application/pdf")]
    public async Task<IActionResult> GenerateApprovalPdf([FromBody] ApprovalLetterModel model)
    {
        _logger.LogInformation("Generating Approval PDF for member: {MemberId}", model.Member.MemberId);

        var pdfBytes = await _letterService.GenerateLetterPdfAsync("ApprovalLetter", model);

        return File(pdfBytes, "application/pdf",
            $"Approval_{model.LetterReferenceId}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpPost("approval/preview")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewApprovalHtml([FromBody] ApprovalLetterModel model)
    {
        var html = await _letterService.GenerateLetterHtmlAsync("ApprovalLetter", model);
        return Content(html, "text/html");
    }

    [HttpGet("approval/sample")]
    [Produces("application/pdf")]
    public async Task<IActionResult> GenerateSampleApprovalPdf()
    {
        var model = SampleDataFactory.CreateSampleApprovalLetter();
        var pdfBytes = await _letterService.GenerateLetterPdfAsync("ApprovalLetter", model);
        return File(pdfBytes, "application/pdf", $"SampleApproval_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("approval/sample/preview")]
    [Produces("text/html")]
    public async Task<IActionResult> PreviewSampleApprovalHtml()
    {
        var model = SampleDataFactory.CreateSampleApprovalLetter();
        var html = await _letterService.GenerateLetterHtmlAsync("ApprovalLetter", model);
        return Content(html, "text/html");
    }

    // ═══════════════════════════════════════════════════════════════════
    //  LIST AVAILABLE TEMPLATES
    // ═══════════════════════════════════════════════════════════════════

    [HttpGet("templates")]
    public IActionResult ListTemplates()
    {
        var templateDir = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
        if (!Directory.Exists(templateDir))
            return Ok(new { templates = Array.Empty<string>() });

        var templates = Directory.GetFiles(templateDir, "*.cshtml")
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        return Ok(new { templates });
    }
}
