using LetterGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace LetterGenerator.Engine.PdfGeneration;

/// <summary>
/// Generates PDFs by rendering HTML in a headless Chromium browser via Playwright.
/// 
/// Why Playwright?
/// - Microsoft-maintained, MIT license (enterprise-friendly)
/// - Pixel-perfect rendering — same engine as Chrome
/// - Supports @media print CSS, web fonts, SVG, base64 images
/// - Headers/footers with page numbers
/// </summary>
public class PlaywrightPdfGenerator : IPdfGenerator, IAsyncDisposable
{
    private readonly ILogger<PlaywrightPdfGenerator> _logger;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public PlaywrightPdfGenerator(ILogger<PlaywrightPdfGenerator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Lazy-initialize Playwright and launch the browser once.
    /// The browser instance is reused across requests for performance.
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            _logger.LogInformation("Initializing Playwright and launching Chromium...");
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            _initialized = true;
            _logger.LogInformation("Playwright Chromium browser launched successfully.");
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<byte[]> GeneratePdfAsync(string htmlContent, PdfOptions? options = null)
    {
        await EnsureInitializedAsync();

        options ??= new PdfOptions();

        _logger.LogDebug("Generating PDF. Format: {Format}, Landscape: {Landscape}",
            options.Format, options.Landscape);

        // Create a new page (isolated context) for each PDF
        var page = await _browser!.NewPageAsync();
        try
        {
            // Load the HTML content
            await page.SetContentAsync(htmlContent, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Wait for any images to fully render
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Generate PDF

            var pdfBytes = await page.PdfAsync(new PagePdfOptions
            {
                Format = options.Format,
                Landscape = options.Landscape,
                PrintBackground = options.PrintBackground,
                Margin = new()
                {
                    Top = options.MarginTop,
                    Bottom = options.MarginBottom,
                    Left = options.MarginLeft,
                    Right = options.MarginRight
                },
                DisplayHeaderFooter = options.DisplayHeaderFooter,
                HeaderTemplate = options.HeaderTemplate ?? "<span></span>",
                FooterTemplate = options.FooterTemplate ?? "<span></span>"
            });

            _logger.LogInformation("PDF generated successfully. Size: {Size} bytes", pdfBytes.Length);
            return pdfBytes;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }

        _playwright?.Dispose();
        _playwright = null;
        _initialized = false;

        GC.SuppressFinalize(this);
    }
}
