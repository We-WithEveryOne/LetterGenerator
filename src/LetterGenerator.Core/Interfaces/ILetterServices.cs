namespace LetterGenerator.Core.Interfaces;

/// <summary>
/// Renders a Razor template with a strongly-typed model into HTML.
/// </summary>
public interface IRazorTemplateRenderer
{
    /// <summary>
    /// Renders a .cshtml template file with the given model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="templateName">Template file name (without extension), e.g., "DenialLetter".</param>
    /// <param name="model">The data model to bind.</param>
    /// <returns>Rendered HTML string.</returns>
    Task<string> RenderAsync<TModel>(string templateName, TModel model) where TModel : class;
}

/// <summary>
/// Converts rendered HTML into a PDF byte array.
/// </summary>
public interface IPdfGenerator
{
    /// <summary>
    /// Generates a PDF from the provided HTML content.
    /// </summary>
    /// <param name="htmlContent">Fully rendered HTML.</param>
    /// <param name="options">Optional PDF generation settings.</param>
    /// <returns>PDF as a byte array.</returns>
    Task<byte[]> GeneratePdfAsync(string htmlContent, PdfOptions? options = null);
}

/// <summary>
/// High-level service that combines template rendering + PDF generation.
/// </summary>
public interface ILetterService
{
    Task<byte[]> GenerateLetterPdfAsync<TModel>(string templateName, TModel model, PdfOptions? options = null) where TModel : class;
    Task<string> GenerateLetterHtmlAsync<TModel>(string templateName, TModel model) where TModel : class;
}

/// <summary>
/// Options controlling PDF output.
/// </summary>
public class PdfOptions
{
    public string Format { get; set; } = "Letter";           // Letter, A4, Legal
    public string MarginTop { get; set; } = "0.75in";
    public string MarginBottom { get; set; } = "0.75in";
    public string MarginLeft { get; set; } = "0.75in";
    public string MarginRight { get; set; } = "0.75in";
    public bool PrintBackground { get; set; } = true;
    public bool Landscape { get; set; } = false;
    public string? HeaderTemplate { get; set; }
    public string? FooterTemplate { get; set; }
    public bool DisplayHeaderFooter { get; set; } = false;
}
