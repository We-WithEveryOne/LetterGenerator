using LetterGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace LetterGenerator.Engine.Services;

/// <summary>
/// Orchestrates the letter generation pipeline:
/// 1. Render Razor template with model → HTML
/// 2. Convert HTML → PDF via Playwright
/// 
/// This is the single service your API controllers and batch jobs call.
/// </summary>
public class LetterService : ILetterService
{
    private readonly IRazorTemplateRenderer _renderer;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly ILogger<LetterService> _logger;

    public LetterService(
        IRazorTemplateRenderer renderer,
        IPdfGenerator pdfGenerator,
        ILogger<LetterService> logger)
    {
        _renderer = renderer;
        _pdfGenerator = pdfGenerator;
        _logger = logger;
    }

    public async Task<string> GenerateLetterHtmlAsync<TModel>(
        string templateName, TModel model) where TModel : class
    {
        _logger.LogInformation("Generating HTML for template: {Template}", templateName);
        return await _renderer.RenderAsync(templateName, model);
    }

    public async Task<byte[]> GenerateLetterPdfAsync<TModel>(
        string templateName, TModel model, PdfOptions? options = null) where TModel : class
    {
        _logger.LogInformation("Generating PDF for template: {Template}", templateName);

        // Step 1: Render Razor → HTML
        var html = await _renderer.RenderAsync(templateName, model);

        // Step 2: HTML → PDF via Playwright
        var pdfBytes = await _pdfGenerator.GeneratePdfAsync(html, options);

        _logger.LogInformation("PDF generated. Template: {Template}, Size: {Size} bytes",
            templateName, pdfBytes.Length);

        return pdfBytes;
    }
}
